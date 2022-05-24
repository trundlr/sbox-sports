using Sports.Football;
using Sports.Football.States;

namespace Sports;

[HammerEntity]
[Library( "sports_football_gamemode" )]
[EditorModel( "models/football/football_ball.vmdl" )]
[Title( "Football Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class FootballGame : BaseGamemode
{
	public FBStateMachine FootballStateMachine => StateMachine as FBStateMachine;



	public override BasePlayer CreatePawn() => new FootballPlayer();

	[Property, FGDType( "target_destination" )]
	public string BallSpawnerName { get; set; }

	public Ball CurrentBall { get; set; }

	public void OnGoal( Goal goal )
	{
		FootballStateMachine.Goal = true;
	}

	public override void Spawn()
	{
		base.Spawn();

		SetStateMachine<FBStateMachine>( nameof( FBWarmupState ) );
	}

	public override void OnStart()
	{
		base.OnStart();

		StateMachine?.SetState( nameof( FBPreGameState ) );
	}

	public override void OnFinish()
	{
		base.OnFinish();

		StateMachine?.SetState( nameof( FBWarmupState ) );

		if ( CurrentBall.IsValid() && IsServer )
		{
			CurrentBall.Delete();
		}
	}

	[ConCmd.Server]
	public static void StartFootball()
	{
		if ( ConsoleSystem.Caller is not Client cl || cl.GetGamemode() is not FootballGame game )
			return;

		game.Start();
	}

	[ConCmd.Server]
	public static void EndFootball()
	{
		if ( ConsoleSystem.Caller is not Client cl || cl.GetGamemode() is not FootballGame game )
			return;

		game.Finish();
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		if ( CurrentBall.IsValid() )
			CurrentBall?.Simulate( cl );
	}

	public void SpawnBall()
	{
		if ( CurrentBall.IsValid() && IsServer )
		{
			CurrentBall.Delete();
		}

		if ( !string.IsNullOrEmpty( BallSpawnerName ) )
		{
			var ballSpawner = Entity.FindByName( BallSpawnerName ) as BallSpawner;

			CurrentBall = ballSpawner.SpawnBall();
		}
	}
}
