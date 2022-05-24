using Sandbox;
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
	public TurnStateMachine TurnStateMachine => StateMachine as TurnStateMachine;
	public override BasePlayer CreatePawn() => new BowlingPlayer();

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

	public override void DressPlayer( BasePlayer pawn, SportsClothingContainer container )
	{
		// TODO: Get the clothing files from our own clothing collection later on - players will want to be able to select which variant they use.
		container.AddRange( new Clothing[] {
			ResourceLibrary.Get<Clothing>( "resources/clothing/bowling/glove_apetavern.clothing" ),
			ResourceLibrary.Get<Clothing>( "resources/clothing/bowling/shoes_france.clothing" ),
		} );
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
