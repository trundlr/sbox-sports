using Sports.StateSystem;

namespace Sports;

[HammerEntity]
[Library( "sports_boxing_gamemode" )]
[EditorModel( "models/sportscitizen/clothes/boxing/boxinggloves.vmdl" )]
[Title( "Boxing Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class Boxing : BaseGamemode, IStateMachine<TurnStateMachine>
{
	[Net]
	public TurnStateMachine StateMachine { get; set; }

	public override BasePlayer CreatePawn() => new BoxingPlayer();

	public override void Spawn()
	{
		base.Spawn();

		StateMachine = new()
		{
			Gamemode = this,
		};

		StateMachine.SetState( nameof( LobbyState ) );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();

		if ( Host.IsServer )
			StateMachine?.Delete();
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		StateMachine?.Simulate( cl );

		if ( !Debug.Enabled )
			return;

		// End turn of yourself
		if ( Input.Pressed( InputButton.Duck ) && !cl.IsBot )
		{
			cl.EndTurn();
		}
		// End any player's turn
		if ( Input.Pressed( InputButton.View ) )
		{
			StateMachine?.EndTurn();
		}
	}

	public override void OnStart()
	{
		base.OnStart();

		StateMachine?.StartGame();
	}

	public override void OnFinish()
	{
		base.OnFinish();

		StateMachine?.EndGame();
	}

	[ConCmd.Server]
	public static void StartBoxing()
	{
		if ( ConsoleSystem.Caller is not Client cl || cl.GetGamemode() is not Boxing game )
			return;

		game.Start();
	}

	[ConCmd.Server]
	public static void EndBoxing()
	{
		if ( ConsoleSystem.Caller is not Client cl || cl.GetGamemode() is not Boxing game )
			return;

		game.Finish();
	}
}
