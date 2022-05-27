namespace Sports;

public partial class BowlingBallCarriable : BaseCarriable
{
	public BowlingBall BowlingBall { get; set; }

	private BowlingPlayer BowlingPlayer => Owner as BowlingPlayer;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/bowling/bowlingball.vmdl" );
		Tags.Add( "bowling_owner" );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( Debug.Enabled )
		{
			DebugOverlay.ScreenText( "[BOWLING CARRIABLE]\n" +
						$"TimeSinceLastThrow:             {BowlingPlayer?.TimeSinceLastThrow}\n" +
						$"Active ball:                    {BowlingBall}\n" +
						$"Active ball pos:                {BowlingBall?.Position}", 24 );
		}

		BowlingBall?.Simulate( cl );

		if ( CanThrow() )
			Throw();
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
		BowlingBall.Velocity = Parent.Rotation.Forward * 420;
		BowlingBall.AngularDirection = Parent.Rotation.Left * 15;
		BowlingBall.Owner = this;

		EnableDrawing = false;
	}

	public void Reset()
	{
		EnableDrawing = true;
	}

	private bool CanThrow()
	{
		if ( BowlingPlayer.TimeSinceLastThrow < 5 )
			return false;

		if ( !Input.Pressed( InputButton.PrimaryAttack ) )
			return false;

		return true;
	}

	private void Throw()
	{
		// pawn has thrown
		BowlingPlayer.HasThrown = true;
		BowlingPlayer.TimeSinceLastThrow = 0;

		// Tell the animator that we want to throw our ball.
		BowlingPlayer.PlayerAnimator.DoThrow();
	}
}
