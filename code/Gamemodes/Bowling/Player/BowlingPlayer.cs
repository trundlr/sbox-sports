namespace Sports;

public class BowlingPlayer : BasePlayer
{
	public override void Respawn()
	{
		base.Respawn();

		SetModel( "models/sportscitizen/citizen_bowling.vmdl" );
		Camera = new ThirdPersonCamera();

		ActiveChild = new BowlingBallCarriable();
		ActiveChild.OnCarryStart( this );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateActiveChild( cl, ActiveChild );

		DebugOverlay.ScreenText( "[BOWLING PAWN]\n" +
			$"ActiveChild:                    {ActiveChild}", 4 );
	}
}
