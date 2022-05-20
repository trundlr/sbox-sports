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
	public TurnStateMachine TurnStateMachine => Components.Get<TurnStateMachine>();
	public override BasePlayer CreatePawn()
	{
		return new BowlingPlayer();
	}
	public override void Spawn()
	{
		base.Spawn();
		Components.GetOrCreate<TurnStateMachine>();
		TurnStateMachine.CurrentState = new LobbyState(); // this can be turned into a specific Game State to allow different States.
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

	public override void OnStart()
	{
		base.OnStart();

		TurnStateMachine.StartGame();
	}
	public override void OnFinish()
	{
		base.OnFinish();
		TurnStateMachine.EndGame();
	}
}
