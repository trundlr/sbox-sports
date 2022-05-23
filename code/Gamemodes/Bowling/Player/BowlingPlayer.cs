namespace Sports;

public partial class BowlingPlayer : BasePlayer
{
	[Net, Predicted]
	public bool HasThrown { get; set; }

	[Net, Predicted]
	public bool TurnEnded { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceLastThrow { get; set; }

	public BowlingPlayerAnimator PlayerAnimator => GetActiveAnimator() as BowlingPlayerAnimator;

	[Net, Predicted]
	private TimeSince TimeSinceTurnEnded { get; set; }

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
		Log.Debug( $"Bowling turn started for: {Client.Name}" );

		// TODO: Face your assigned alley once your turn begins. This is needs to be replaced later when we have alley entities.
		Rotation = Rotation.FromYaw( 90 );

		if ( IsServer )
			ActiveChild?.Delete();
		ActiveChild = new BowlingBallCarriable();
		ActiveChild.OnCarryStart( this );

		// reset turn
		HasThrown = false;
		TurnEnded = false;

		// set pos to gamemode pos if we can get it (temp reset)
		var pos = Client.GetGamemode()?.Position;
		Position = pos ?? Position;
	}

	/// <summary>
	/// Called when this players turn has ended.
	/// </summary>
	public void OnTurnEnded( bool wasGoodBowl )
	{
		Log.Debug( $"Bowling turn ended for: {Client.Name}" );

		PlayerAnimator.DoResultAnimation( wasGoodBowl );

		TimeSinceTurnEnded = 0;

		TurnEnded = true;
	}

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		ActiveChild?.OnAnimEventGeneric( name, intData, floatData, vectorData, stringData );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		SimulateMovement( cl );

		SimulateActiveChild( cl, ActiveChild );

		// temp to restart turn, this should be handled by the game turn machine
		if ( TurnEnded && TimeSinceTurnEnded > 1.5f )
			OnTurnStarted();

		// TODO: use the pit trigger as time and pingroup as good bowl identifier for this.
		if ( !TurnEnded && HasThrown && TimeSinceLastThrow > 3f )
			OnTurnEnded( true );

		if ( Debug.Enabled )
		{
			DebugOverlay.ScreenText( "[BOWLING PAWN]\n" +
			$"ActiveChild:					{ActiveChild}\n" +
			$"HasThrown:					{HasThrown}\n" +
			$"TurnEnded:                    {TurnEnded}\n" +
			$"TimeSinceTurnEnded:			{TimeSinceTurnEnded}\n", 4 );
		}
	}
}
