using Sports.BotSystem;
using Sports.StateSystem;

namespace Sports;

public abstract partial class BaseGamemode : Entity
{
	[Net]
	public IList<Client> Clients { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		SportsGame.Instance?.Gamemodes.Add( this );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		SportsGame.Instance?.Gamemodes.Remove( this );

		if ( Host.IsServer )
			StateMachine?.Delete();
	}

	public override void Simulate( Client cl )
	{
		StateMachine?.Simulate( cl );
	}

	public override void FrameSimulate( Client cl )
	{
		StateMachine?.FrameSimulate( cl );
	}

	public void Start()
	{
		StateMachine?.OnGamemodeStart();
		OnStart();
	}

	public void Finish()
	{
		StateMachine?.OnGamemodeEnd();
		OnFinish();
	}

	public void AddClient( Client cl )
	{
		if ( !CanAddClient( cl ) )
		{
			Log.Debug( $"Sports: {cl.Name}'s was refused to join gamemode: {Name}" );

			return;
		}

		Clients.Add( cl );

		Log.Debug( $"Sports: Adding {cl.Name} to gamemode: {Name}" );

		var component = cl.GetGamemodeComponent();
		component.Gamemode = this;

		cl.Pawn?.Delete();

		var pawn = CreatePawn();
		cl.Pawn = pawn;
		pawn.InitialSpawn();

		Log.Debug( $"Sports: {cl.Name}'s pawn: {cl.Pawn}" );

		UI.SportsChatBox.AddInformation( To.Single( cl ), $"You joined {Name}" );

		OnClientAdded( cl );
	}

	public void RemoveClient( Client cl, LeaveReason reason = LeaveReason.Leave )
	{
		Clients.Remove( cl );

		Log.Debug( $"Sports: {cl.Name}' was removed from gamemode: {Name} with reason: {reason}" );

		UI.SportsChatBox.AddInformation( To.Single( cl ), $"You left {Name} with reason: {reason}" );

		var component = cl.GetGamemodeComponent();
		component.Gamemode = null;

		// Go back to default pawn
		SportsGame.Instance?.SetupDefaultPawn( cl );

		OnClientRemoved( cl, reason );
	}

	public void AddBot( SportsBot bot )
	{
		AddClient( bot.Client );
		OnBotAdded( bot );
	}

	/// <summary>
	/// Decides which hud panel to display when a part of this gamemode
	/// </summary>
	/// <returns></returns>
	public virtual Panel CreateHud() => null;

	/// <summary>
	/// The gamemode's specified Pawn
	/// </summary>
	/// <returns></returns>
	public virtual BasePlayer CreatePawn() => new BasePlayer();

	/// <summary>
	/// Called when the gamemode starts, normally by having enough players
	/// </summary>
	public virtual void OnStart()
	{
	}

	/// <summary>
	/// Called when the gamemode is "finished"
	/// normally when the flow of the gamemode has ended for any reason
	/// </summary>
	public virtual void OnFinish()
	{
	}

	/// <summary>
	/// Called when a client has joined the gamemode
	/// </summary>
	/// <param name="cl"></param>
	public virtual void OnClientAdded( Client cl )
	{
	}

	/// <summary>
	/// Called when a bot has joined the gamemode
	/// </summary>
	/// <param name="bot"></param>
	public virtual void OnBotAdded( SportsBot bot )
	{
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
	public virtual void OnClientRemoved( Client cl, LeaveReason reason = LeaveReason.Leave )
	{
	}

	/// <summary>
	/// Called when a bot leaves a gamemode. We can use this to clean up and alter the gamemode's state if required.
	/// </summary>
	/// <param name="bot"></param>
	public virtual void OnBotRemoved( SportsBot bot )
	{
	}

	/// <summary>
	/// Called when a pawn is damaged while in a gamemode
	/// </summary>
	/// <param name="pawn"></param>
	/// <param name="dmg"></param>
	public virtual void OnPawnDamaged( BasePlayer pawn, DamageInfo dmg )
	{
	}

	/// <summary>
	/// Called when a pawn is killed while in a gamemode
	/// </summary>
	/// <param name="pawn"></param>
	/// <param name="lastdmg"></param>
	public virtual void OnPawnKilled( BasePlayer pawn, DamageInfo lastdmg )
	{
	}

	/// <summary>
	/// Called to dictate where the player's pawn is moved to upon spawn
	/// </summary>
	/// <param name="pawn"></param>
	public virtual void MovePawnToSpawnpoint( BasePlayer pawn )
	{
		// Default gamemode behavior will move players to the gamemode entity's origin.
		pawn.Position = Position;
	}

	/// <summary>
	/// Called to dress the pawn with the client's selected clothing
	/// </summary>
	/// <param name="pawn"></param>
	/// <param name="container"></param>
	public virtual void DressPlayer( BasePlayer pawn, SportsClothingContainer container ) { }

	[ConCmd.Server( "sports_gamemode_leave" )]
	protected static void LeaveGamemode()
	{
		var cl = ConsoleSystem.Caller;

		cl?.GetGamemode()?.RemoveClient( cl, LeaveReason.Leave );
	}
}
