using Sports.StateSystem;

namespace Sports;

[HammerEntity]
[Library( "sports_bowling_gamemode" )]
[EditorModel( "models/Bowling/bpin.vmdl" )]
[Title( "Bowling Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class Bowling : BaseGamemode, ITurnStateMachine
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

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		TurnStateMachine?.Simulate( cl );


		if ( !Debug.Enabled )
			return;
		//End turn of yourself
		if ( Input.Pressed( InputButton.Duck ) && !cl.IsBot )
		{
			cl.EndTurn();
		}
		//End any player's turn
		if ( Input.Pressed( InputButton.View ) )
		{
			TurnStateMachine?.EndTurn();
		}

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
}
