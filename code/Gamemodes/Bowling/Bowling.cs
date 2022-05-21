using Sports.StateSystem;

namespace Sports;

[HammerEntity]
[Library( "sports_bowling_gamemode" )]
[EditorModel( "models/Bowling/bpin.vmdl" )]
[Title( "Bowling Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class Bowling : BaseGamemode
{
	[Net] public TurnStateMachine TurnStateMachine { get; set; }
	public override BasePlayer CreatePawn()
	{
		return new BowlingPlayer();
	}
	public override void Spawn()
	{
		base.Spawn();
		TurnStateMachine = new()
		{
			Gamemode = this,
		};
		TurnStateMachine.SetState( nameof( LobbyState ) );
	}
	protected override void OnDestroy()
	{
		base.OnDestroy();
		if ( Host.IsServer )
			TurnStateMachine?.Delete();
	}

	[ConCmd.Server]
	public static void StartBowling()
	{
		if ( ConsoleSystem.Caller is not Client cl || cl.GetGamemode() is not Bowling game )
			return;

		game.Start();
	}
	[ConCmd.Server]
	public static void EndBowling()
	{
		if ( ConsoleSystem.Caller is not Client cl || cl.GetGamemode() is not Bowling game )
			return;

		game.Finish();
	}
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		TurnStateMachine?.Simulate( cl );
	}

	public override void OnStart()
	{
		base.OnStart();

		TurnStateMachine?.StartGame();
	}
	public override void OnFinish()
	{
		base.OnFinish();
		TurnStateMachine?.EndGame();
	}
}
