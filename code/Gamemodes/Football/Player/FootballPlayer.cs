using System.Linq;
using Sports.Football;

namespace Sports;

public partial class FootballPlayer : BasePlayer
{

	public FootballPlayerAnimator PlayerAnimator => GetActiveAnimator() as FootballPlayerAnimator;

	public override void Respawn()
	{
		base.Respawn();
		SetModel( "models/sportscitizen/citizen_football.vmdl" );

		Camera = new ThirdPersonCamera();
		Controller = new WalkController();
		Animator = new FootballPlayerAnimator();

		Tags.Add( "player" );
	}

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		ActiveChild?.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );

		if ( name == "kick" )
		{
			KickBall( (FootballPlayerAnimator.KickDirection)intData );
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		if ( Input.Pressed( InputButton.PrimaryAttack ) )
		{
			PlayerAnimator.DoKick( FootballPlayerAnimator.KickDirection.Forward );
		}
		if ( Input.Pressed( InputButton.Use ) )
		{
			PlayerAnimator.DoKick( FootballPlayerAnimator.KickDirection.Right );
		}
		if ( Input.Pressed( InputButton.Menu ) )
		{
			PlayerAnimator.DoKick( FootballPlayerAnimator.KickDirection.Left );
		}

		if ( Debug.Enabled )
		{
			DebugOverlay.ScreenText( "[FOOTBALL PAWN]\n" +
			$"ActiveChild:                    {ActiveChild}", 4 );
		}
	}

	public void KickBall( FootballPlayerAnimator.KickDirection kickDirection )
	{
		var traceposition = Position + Vector3.Up * 32;
		var tr = Trace.Sphere( 25, traceposition, traceposition + Rotation.Forward * 100 ).Ignore( this ).EntitiesOnly().Run();

		if ( Debug.Enabled )
			DebugOverlay.TraceResult( tr, 4 );

		if ( tr.Hit )
		{
			if ( tr.Entity is SoccerBall ball )
			{

				var BallKickDirection = kickDirection switch
				{
					FootballPlayerAnimator.KickDirection.Left => Rotation.Left * 1000,
					FootballPlayerAnimator.KickDirection.Right => Rotation.Right * 1000,
					_ => Rotation.Forward * 1000
				};

				ball.Velocity += BallKickDirection;
				ball.Velocity += Velocity + Vector3.Up * 100;

				if ( IsServer )
				{
					ball.Position += Vector3.Up * 1;
					ball.GroundEntity = null;
				}

				ball.SetActivePlayer( this );
			}

		}

	}
}
