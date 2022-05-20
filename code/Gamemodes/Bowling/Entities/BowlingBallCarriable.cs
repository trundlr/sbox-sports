namespace Sports;

public partial class BowlingBallCarriable : BaseCarriable
{
	public BowlingBall BowlingBall { get; set; }

	[Net, Predicted] TimeSince timeSinceLastThrow { get; set; }

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

	public override void OnAnimEventGeneric( string name, int intData, float floatData, Vector3 vectorData, string stringData )
	{
		if ( !IsServer )
			return;
		if ( name == "release" )//String compare is pretty rough tbh
		{
			BowlingBall?.Delete();
			BowlingBall = new();

			BowlingBall.Position = Position;//Set it to the position in your hand (might need tweaking though, bit shit to rely on animated position for this?)
			BowlingBall.Velocity = Parent.Rotation.Forward * 512;//Use player rotation not eye rotation
			BowlingBall.Owner = this;

			EnableDrawing = false;

			timeSinceLastThrow = 0;
		}
	}

	private bool CanThrow()
	{
		if ( timeSinceLastThrow > 3f )//Just a dumb trigger positive hit after 3 seconds thing
			(Owner as AnimatedEntity)?.SetAnimParameter( "b_bowling_positive_hit", true );


		if ( IsServer && timeSinceLastThrow > 4.5f )//Turn yourself back on after 1.5 seconds
			EnableDrawing = true;


		if ( timeSinceLastThrow < 5 )
			return false;


		if ( !Input.Pressed( InputButton.PrimaryAttack ) )
			return false;

		return true;
	}

	private void Throw()
	{
		(Owner as AnimatedEntity)?.SetAnimParameter( "b_bowling_throw", true );

		if ( !IsServer )
			return;

	}
}
