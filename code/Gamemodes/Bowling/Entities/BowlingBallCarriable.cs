namespace Sports;

public partial class BowlingBallCarriable : BaseCarriable
{
	public BowlingBall BowlingBall { get; set; }
	[Net, Predicted] TimeSince TimeSinceLastThrow { get; set; }
	BowlingPlayer BowlingPlayer => Owner as BowlingPlayer;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/bowling/bowlingball.vmdl" );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Debug.Enabled )
		{
			DebugOverlay.ScreenText( "[BOWLING CARRIABLE]\n" +
						$"TimeSinceLastThrow:             {TimeSinceLastThrow}\n" +
						$"Active ball:                    {BowlingBall}\n" +
						$"Active ball pos:                {BowlingBall?.Position}", 24 );
		}

		BowlingBall?.Simulate( cl );

		if ( CanThrow() )
			Throw();

		// TODO: Remove this later once we determine bowl success criteria.
		if ( TimeSinceLastThrow > 3f )
			BowlingPlayer.OnTurnEnded( true );

		// TODO: When the anim event is added, re-enable drawing of this once the celebration has finished.
		if ( TimeSinceLastThrow > 4.5f )
			EnableDrawing = true;
	}

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		if ( name == "release" )
			ReleaseBall();
	}

	/// <summary>
	/// What should happen when the ball is released from the players hand.
	/// </summary>
	private void ReleaseBall()
	{
		if ( IsServer )
		{
			BowlingBall?.Delete();
			BowlingBall = new();
		}

		if ( BowlingBall is null )
			return;

		BowlingBall.Position = Position;
		BowlingBall.Velocity = Parent.Rotation.Forward * 512;
		BowlingBall.Owner = this;

		EnableDrawing = false;
	}

	private bool CanThrow()
	{
		if ( TimeSinceLastThrow < 5 )
			return false;

		if ( !Input.Pressed( InputButton.PrimaryAttack ) )
			return false;

		return true;
	}

	private void Throw()
	{
		// Tell the animator that we want to throw our ball.
		BowlingPlayer.PlayerAnimator.DoThrow();

		TimeSinceLastThrow = 0;
	}
}
