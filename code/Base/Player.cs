namespace Sports;

public partial class BasePlayer : AnimatedEntity, IInteractable
{
	public virtual float RespawnTime => 1;

	public virtual float MaxHealth => 100;

	[Net]
	public TimeSince TimeSinceDied { get; set; }

	[Net, Predicted]
	public BaseCarriable LastActiveChild { get; set; }

	[Net, Predicted]
	public BaseCarriable ActiveChild { get; set; }

	public DamageInfo LastDamageInfo { get; protected set; }

	public BaseGamemode CurrentGamemode => Client.GetGamemode();

	public SportsClothingContainer SportsClothingContainer { get; protected set; }

	public override void Simulate( Client cl )
	{
		if ( LifeState == LifeState.Dead )
		{
			if ( TimeSinceDied > RespawnTime && IsServer )
			{
				Respawn();
			}
		}

		SimulateActiveChild( cl, ActiveChild );
		GetActiveController()?.Simulate( cl, this, GetActiveAnimator() );
	}

	public virtual void InitialSpawn()
	{
		SportsClothingContainer = new();
		SportsClothingContainer.LoadFromClient( Client );
		Respawn();
		CurrentGamemode?.DressPlayer( this, SportsClothingContainer );
		SportsClothingContainer.DressEntity( this );
	}

	public virtual void Respawn()
	{
		Host.AssertServer();

		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		Camera = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();

		LifeState = LifeState.Alive;
		Health = MaxHealth;
		Velocity = Vector3.Zero;

		CreateHull();
		ResetInterpolation();

		var gamemode = CurrentGamemode;
		if ( gamemode.IsValid() )
		{
			// Decide spawn for when not in a gamemode
			gamemode.MovePawnToSpawnpoint( this );
		}
		else
		{
			// Decide spawn for when not in a gamemode
			SportsGame.Instance?.MoveToSpawnpoint( this );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		LastDamageInfo = info;

		CurrentGamemode?.OnPawnDamaged( this, info );
	}

	public override void OnKilled()
	{
		base.OnKilled();

		CurrentGamemode?.OnPawnKilled( this, LastDamageInfo );
	}

	public virtual void SimulateActiveChild( Client client, BaseCarriable child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( LastActiveChild.IsAuthority )
		{
			LastActiveChild.Simulate( client );
		}
	}

	public virtual void OnActiveChildChanged( BaseCarriable previous, BaseCarriable next )
	{
		previous?.ActiveEnd( this, previous.Owner != this );
		next?.ActiveStart( this );
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		GetActiveController()?.FrameSimulate( cl, this, GetActiveAnimator() );
	}

	public virtual void CreateHull()
	{
		CollisionGroup = CollisionGroup.Player;
		AddCollisionLayer( CollisionLayer.Player );
		SetupPhysicsFromAABB( PhysicsMotionType.Keyframed, new Vector3( -16, -16, 0 ), new Vector3( 16, 16, 72 ) );

		MoveType = MoveType.MOVETYPE_WALK;
		EnableHitboxes = true;
	}

	public override void BuildInput( InputBuilder input )
	{
		if ( input.StopProcessing )
			return;

		ActiveChild?.BuildInput( input );

		GetActiveController()?.BuildInput( input );

		if ( input.StopProcessing )
			return;

		GetActiveAnimator()?.BuildInput( input );
	}

	public IEnumerable<Interaction> GetInteractions()
	{
		return new List<Interaction>()
		{
			new PartyInviteInteraction( this ),
			new PartyKickInteraction( this )
		};
	}

	//
	// Controller
	//

	[Net, Predicted]
	public PawnController Controller { get; set; }

	[Net]
	public PawnController DevController { get; set; }

	public virtual PawnController GetActiveController()
	{
		return DevController ?? Controller;
	}

	//
	// Animator
	//

	[Net, Predicted]
	protected PawnAnimator Animator { get; set; }

	public virtual PawnAnimator GetActiveAnimator() => Animator;

	//
	// Camera
	//

	public CameraMode Camera
	{
		get => Components.Get<CameraMode>();
		set
		{
			var current = Camera;
			if ( current == value ) return;

			Components.RemoveAny<CameraMode>();
			Components.Add( value );
		}
	}
}
