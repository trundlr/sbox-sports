using Sandbox;

namespace Sports;

public partial class BowlingPlayer : BasePlayer
{
	[Net] public bool HasThrown { get; set; }
	public BowlingPlayerAnimator PlayerAnimator => GetActiveAnimator() as BowlingPlayerAnimator;

	public override void Respawn()
	{
		base.Respawn();

		SetModel( "models/sportscitizen/citizen_bowling.vmdl" );

		Camera = new ThirdPersonCamera();
		Animator = new BowlingPlayerAnimator();
		Controller = new PawnController();

		// This should be fired from the state handler once implemented.
		OnTurnStarted();
	}

	/// <summary>
	/// Called when this players turn has started.
	/// </summary>
	public void OnTurnStarted()
	{
		// TODO: Face your assigned alley once your turn begins. This is needs to be replaced later when we have alley entities.
		Rotation = Rotation.FromYaw( 90 );

		ActiveChild = new BowlingBallCarriable();
		ActiveChild.OnCarryStart( this );

		HasThrown = false;
	}

	/// <summary>
	/// Called when this players turn has ended.
	/// </summary>
	public void OnTurnEnded( bool wasGoodBowl )
	{
		PlayerAnimator.DoResultAnimation( wasGoodBowl );

		HasThrown = false;
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
			DebugOverlay.ScreenText( "[BOWLING PAWN]\n" +
			$"ActiveChild:                    {ActiveChild}", 4 );
		}
	}
}
