namespace Sports;

public abstract partial class BaseGamemode : Entity
{
	public override void Spawn()
	{
		base.Spawn();

		// for the inspector
		Name = ClassInfo.Name;

		Transmit = TransmitType.Always;

		SportsGame.Instance?.Gamemodes.Add( this );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SportsGame.Instance?.Gamemodes.Remove( this );
	}

	[Net]
	public IList<Client> Clients { get; set; }

	public void Start()
	{
		OnStart();
	}

	public void Finish()
	{
		OnFinish();
	}

	public void AddClient( Client cl )
	{
		if ( !CanAddClient( cl ) )
		{
			return;
		}

		Clients.Add( cl );

		OnClientAdded( cl );
	}

	public void RemoveClient( Client cl, LeaveReason reason = LeaveReason.Leave )
	{
		Clients.Remove( cl );

		OnClientRemoved( cl, reason );
	}

	/// <summary>
	/// Decides which hud panel to display when a part of this gamemode
	/// </summary>
	/// <returns></returns>
	public virtual Panel CreateHud() => null;

	/// <summary>
	/// Called when the gamemode starts, normally by having enough players
	/// </summary>
	public virtual void OnStart() { }

	/// <summary>
	/// Called when the gamemode is "finished"
	/// normally when the flow of the gamemode has ended for any reason
	/// </summary>
	public virtual void OnFinish() { }

	/// <summary>
	/// Called when a client has joined the gamemode
	/// </summary>
	/// <param name="cl"></param>
	public virtual void OnClientAdded( Client cl )
	{
		var component = GamemodeEntityComponent.GetOrCreate( cl );
		component.Gamemode = this;
	}

	/// <summary>
	/// Can we add a client to the gamemode? I.e it's full, or it's in progress (maybe we can delete this)
	/// </summary>
	/// <param name="cl"></param>
	/// <returns></returns>
	public virtual bool CanAddClient( Client cl ) => !Clients.Contains( cl );

	/// <summary>
	/// Called when a client leaves a gamemode. We can use this to clean up and alter the gamemode's state if required.
	/// </summary>
	/// <param name="cl"></param>
	/// <param name="reason"></param>
	public virtual void OnClientRemoved( Client cl, LeaveReason reason = LeaveReason.Leave
	{
		var component = GamemodeEntityComponent.GetOrCreate( cl );
		component.Gamemode = null;
	}

	/// <summary>
	/// Called when a pawn is damaged while in a gamemode
	/// </summary>
	/// <param name="pawn"></param>
	/// <param name="dmg"></param>
	public virtual void OnPawnDamaged( BasePlayer pawn, DamageInfo dmg ) { }

	/// <summary>
	/// Called when a pawn is killed while in a gamemode
	/// </summary>
	/// <param name="pawn"></param>
	/// <param name="lastdmg"></param>
	public virtual void OnPawnKilled( BasePlayer pawn, DamageInfo lastdmg ) { }

	// @TODO: should we remove this?
	public virtual void OnPawnRespawned( BasePlayer pawn ) { }

	// @TODO: should we remove this?
	public virtual void MovePawnToSpawnpoint( BasePlayer pawn ) { }
}
