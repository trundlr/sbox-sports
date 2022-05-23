using Sports.StateSystem;

namespace Sports;

[HammerEntity]
[Library( "sports_boxing_gamemode" )]
[EditorModel( "models/sportscitizen/clothes/boxing/boxinggloves.vmdl" )]
[Title( "Boxing Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class Boxing : BaseGamemode
{
	public TurnStateMachine TurnStateMachine => this.GetStateMachine<TurnStateMachine>();

	public override BasePlayer CreatePawn() => new BoxingPlayer();

	public override void Spawn()
	{
		base.Spawn();

		SetStateMachine<TurnStateMachine>( nameof( LobbyState ) );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

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
			TurnStateMachine?.EndTurn();
		}
	}

	public override void OnStart()
	{
		base.OnStart();

		StateMachine?.SetState( nameof( TurnState ) );
	}

	public override void OnFinish()
	{
		base.OnFinish();

		StateMachine?.SetState( nameof( LobbyState ) );
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
