namespace Sports;

[Solid]
[VisGroup( VisGroup.Trigger )]
[Title( "Gamemode Base Trigger" ), Icon( "select_all" )]
[AutoApplyMaterial( "materials/tools/toolstrigger.vmat" )]
public partial class GamemodeBaseTrigger : GamemodeModelEntity
{
	[ConCmd.Admin( "sports_drawtriggers_toggle" )]
	public static void ToggleDrawTriggers()
	{
		foreach ( var trigger in All.OfType<BaseTrigger>() )
		{
			trigger.DebugFlags ^= EntityDebugFlags.TriggerBounds;
		}
	}

	/// <summary>
	/// Entities with these tags can activate this trigger.
	/// </summary>
	[Property( Title = "Activation Tags" ), DefaultValue( "player" )]
	public TagList ActivationTags { get; set; } = "player";

	/// <summary>
	/// Whether this entity is enabled or not.
	/// </summary>
	[Property]
	public bool Enabled { get; protected set; } = true;

	public IEnumerable<Entity> TouchingEntities => _TouchingEntities;
	public int TouchingEntityCount => _TouchingEntities.Count;

	private readonly List<Entity> _TouchingEntities = new();

	// Used for when an entity enters the trigger while it is disabled, and then the trigger gets enabled
	private readonly List<Entity> _TouchingEntitiesWhileDisabled = new();

	public override void Spawn()
	{
		base.Spawn();

		SetupPhysicsFromModel( PhysicsMotionType.Static );
		CollisionGroup = CollisionGroup.Trigger;
		SetInteractsExclude( CollisionLayer.STATIC_LEVEL | CollisionLayer.WORLD_GEOMETRY | CollisionLayer.PLAYER_CLIP );
		EnableAllCollisions = false;
		EnableTouch = true;

		Transmit = TransmitType.Never;
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( other.IsWorld )
			return;

		AddToucher( other );
	}

	// This is to make sure we can add a toucher after they have entered the trigger but we were on a cooldown or something (trigger_multiple's wait param)
	public override void Touch( Entity other )
	{
		base.Touch( other );

		if ( other.IsWorld )
			return;

		AddToucher( other );
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other.IsWorld )
			return;

		if ( _TouchingEntitiesWhileDisabled.Contains( other ) )
		{
			_TouchingEntitiesWhileDisabled.Remove( other );
		}

		if ( _TouchingEntities.Contains( other ) )
		{
			_TouchingEntities.Remove( other );
			OnTouchEnd( other );

			if ( _TouchingEntities.Count < 1 ) OnTouchEndAll( other );
		}
	}

	protected void AddToucher( Entity toucher )
	{
		if ( !toucher.IsValid() )
			return;

		if ( !Enabled )
		{
			// We don't care about the filter because we will pass these entities to StartTouch
			if ( !_TouchingEntitiesWhileDisabled.Contains( toucher ) )
			{
				_TouchingEntitiesWhileDisabled.Add( toucher );
			}

			return;
		}

		if ( _TouchingEntities.Contains( toucher ) )
			return;

		if ( !PassesTriggerFilters( toucher ) )
			return;

		bool anyoneTouching = _TouchingEntities.Count > 0;

		_TouchingEntities.Add( toucher );
		OnTouchStart( toucher );

		if ( !anyoneTouching ) OnTouchStartAll( toucher );
	}

	/// <summary>
	/// Fired when an entity starts touching this trigger. The touching entity must pass this trigger's filters to cause this output to fire.
	/// </summary>
	protected Output OnStartTouch { get; set; }

	/// <summary>
	/// Fired when an entity stops touching this trigger. Only entities that passed this trigger's filters will cause this output to fire.
	/// </summary>
	protected Output OnEndTouch { get; set; }

	/// <summary>
	/// Fired when an entity starts touching this trigger while no other passing entities are touching it.
	/// </summary>
	protected Output OnStartTouchAll { get; set; }

	/// <summary>
	/// Fired when all entities touching this trigger have stopped touching it.
	/// </summary>
	protected Output OnEndTouchAll { get; set; }

	/// <summary>
	/// Enables this trigger
	/// </summary>
	[Input]
	public void Enable()
	{
		Enabled = true;

		// Everything that is already inside the volume now just started touching us.
		foreach ( var entity in _TouchingEntitiesWhileDisabled.ToArray() )
		{
			if ( !entity.IsValid() ) continue;

			StartTouch( entity );
		}
		_TouchingEntitiesWhileDisabled.Clear();
	}

	/// <summary>
	/// Disables this trigger
	/// </summary>
	[Input]
	public void Disable()
	{
		Enabled = false;

		// We are disabled, pretend all our touching entities stopped touching us.
		foreach ( var entity in TouchingEntities.ToList() )
		{
			if ( !entity.IsValid() ) continue;

			EndTouch( entity );

			if ( !_TouchingEntitiesWhileDisabled.Contains( entity ) )
			{
				_TouchingEntitiesWhileDisabled.Add( entity );
			}
		}
	}

	/// <summary>
	/// Toggles this trigger between enabled and disabled states
	/// </summary>
	[Input]
	public void Toggle()
	{
		if ( Enabled )
		{
			Disable();
		}
		else
		{
			Enable();
		}
	}

	/// <summary>
	///	An entity that passes PassesTriggerFilters has started touching the trigger
	/// </summary>
	public virtual void OnTouchStart( Entity toucher )
	{
		OnStartTouch.Fire( toucher );
	}

	/// <summary>
	///	An entity that started touching this trigger has stopped touching
	/// </summary>
	public virtual void OnTouchEnd( Entity toucher )
	{
		OnEndTouch.Fire( toucher );
	}

	/// <summary>
	///	Called when an entity starts touching this trigger while no other passing entities are touching it
	/// </summary>
	public virtual void OnTouchStartAll( Entity toucher )
	{
		OnStartTouchAll.Fire( toucher );
	}

	/// <summary>
	///	Called when all entities touching this trigger have stopped touching it.
	/// </summary>
	public virtual void OnTouchEndAll( Entity toucher )
	{
		OnEndTouchAll.Fire( toucher );
	}

	/// <summary>
	///	Determine if an entity should be allowed to touch this trigger
	/// </summary>
	public virtual bool PassesTriggerFilters( Entity other )
	{
		if ( other is not ModelEntity )
		{
			return false;
		}

		if ( other.Tags.HasAny( ActivationTags ) || ActivationTags.Contains( "*" ) )
		{
			return true;
		}

		return false;
	}
}
