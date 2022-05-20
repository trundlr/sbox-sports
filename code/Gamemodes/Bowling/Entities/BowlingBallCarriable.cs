namespace Sports;

public class BowlingBallCarriable : BaseCarriable
{
	public BowlingBall BowlingBall { get; set; }

	TimeSince timeSinceLastThrow = 0;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/bowling/bowlingball.vmdl" );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		DebugOverlay.ScreenText( "[BOWLING CARRIABLE]\n" +
			$"TimeSinceLastThrow:             {timeSinceLastThrow}\n" +
			$"Active ball:                    {BowlingBall}\n" +
			$"Active ball pos:                {BowlingBall?.Position}", 24 );

		BowlingBall?.Simulate( cl );

		if ( CanThrow() )
			Throw();
	}

	private bool CanThrow()
	{
		if ( timeSinceLastThrow < 1 )
			return false;

		if ( !Input.Pressed( InputButton.PrimaryAttack ) )
			return false;

		return true;
	}

	private void Throw()
	{
		if ( !IsServer )
			return;

		BowlingBall?.Delete();
		BowlingBall = new();

		var tr = Trace.Ray( Parent.EyePosition, Parent.EyePosition + Parent.EyeRotation.Forward * 64 )
					  .Ignore( Parent )
					  .Run();

		BowlingBall.Position = tr.EndPosition + tr.Normal * 32;
		BowlingBall.Velocity = Parent.EyeRotation.Forward * 512;
		BowlingBall.Owner = this;

		timeSinceLastThrow = 0;
	}
}
