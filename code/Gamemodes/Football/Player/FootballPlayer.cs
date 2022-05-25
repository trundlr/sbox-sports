﻿using Sports.Football;

namespace Sports;

public partial class FootballPlayer : BasePlayer
{
	private TimeSince LastBallTouch;
	public override void Respawn()
	{
		base.Respawn();

		SetModel( "models/sportscitizen/citizen_boxing.vmdl" );

		Camera = new ThirdPersonCamera();
		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
	}

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		ActiveChild?.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );
	}


	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		var tr = Trace.Capsule( new Capsule( 0, Vector3.Up * 100, 20 ), Position + Vector3.Up * 10, Position + Rotation.Forward * 10 ).Ignore( this ).EntitiesOnly().Run();

		if ( tr.Hit && IsServer && LastBallTouch > 0.1 )
		{
			if ( tr.Entity is Ball ball )
			{
				ball.Velocity += Rotation.Forward * 100;
				LastBallTouch = 0;
			}
		}


		if ( Debug.Enabled )
		{
			DebugOverlay.ScreenText( "[FOOTBALL PAWN]\n" +
			$"ActiveChild:                    {ActiveChild}", 4 );
		}
	}
}