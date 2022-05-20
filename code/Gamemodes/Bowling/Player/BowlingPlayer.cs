namespace Sports;

public class BowlingPlayer : BasePlayer
{
	public override void Respawn()
	{
		base.Respawn();

		SetModel( "models/sportscitizen/citizen_bowling.vmdl" );
		Camera = new ThirdPersonCamera();

		Controller = null;//Disable the controller since this player has very limited movement

		Rotation = Rotation.FromYaw( 90 );//This is just the rotation I found works for facing the test course - it's shit, should be handled in custom animator probably.

		ActiveChild = new BowlingBallCarriable();
		ActiveChild.OnCarryStart( this );
	}

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		ActiveChild.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateActiveChild( cl, ActiveChild );

		SetAnimParameter( "move_x", MathX.LerpTo( GetAnimParameterFloat( "move_x" ), Input.Left * -50f, Time.Delta * 10f ) );//Left/right side step

		Position += Rotation * RootMotion * Time.Delta * 1.5f;//Apply root motion to position, should be in controller or animator class instead

		DebugOverlay.ScreenText( "[BOWLING PAWN]\n" +
			$"ActiveChild:                    {ActiveChild}", 4 );
	}
}
