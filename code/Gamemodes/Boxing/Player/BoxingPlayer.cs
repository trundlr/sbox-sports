namespace Sports;

public partial class BoxingPlayer : BasePlayer
{
	public override void Respawn()
	{
		base.Respawn();

		SetModel( "models/sportscitizen/citizen_boxing.vmdl" );

		Camera = new ThirdPersonCamera();
		Controller = new PawnController();
	}

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		ActiveChild?.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateActiveChild( cl, ActiveChild );

		if ( Debug.Enabled )
		{
			DebugOverlay.ScreenText( "[BOXING PAWN]\n" +
			$"ActiveChild:                    {ActiveChild}", 4 );
		}
	}
}
