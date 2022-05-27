namespace Sports;

public partial class BowlingBallCarriable : BaseCarriable
{
	public BowlingBall BowlingBall { get; set; }

	private BowlingPlayer BowlingPlayer => Owner as BowlingPlayer;

	[Net, Predicted]
	public float Strength { get; set; } = 10;

	[Net, Predicted]
	public float Twist { get; set; } = 0;

	public float DefaultTwist => 0.0f;
	/// <summary>
	/// Maximum twist amount for left and right twist.
	/// </summary>
	public float MaximumTwist => 10.0f;

	public float DefaultStrength => 10.0f;
	public float MinimumStrength => 5.0f;
	public float MaximumStrength => 15.0f;

	/// <summary>
	/// Rate at which to change input variables (Strength, Twist)
	/// </summary>
	public float InputRate => 7.0f;
	/// <summary>
	/// Rate at which to change input variables (Strength, Twist) when Shift is held.
	/// </summary>
	public float FineInputRate => 2.3f;

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
						$"TimeSinceLastThrow:			{BowlingPlayer?.TimeSinceLastThrow}\n" +
						$"Active ball:					{BowlingBall}\n" +
						$"Active ball pos:				{BowlingBall?.Position}\n" +
						$"Strength:						{Strength}\n" +
						$"Twist:						{Twist}\n" +
						$"\n" +
						$"", 24 );
		}

		BowlingBall?.Simulate( cl );

		if ( CanThrow() )
		{
			Throw();
			return;
		}

		if ( BowlingPlayer.HasThrown )
			return;

		var rate = Input.Down( InputButton.Run ) ? FineInputRate : InputRate;

		// temp strength and twist controls
		Strength += Input.Forward * Time.Delta * rate;
		Strength = Strength.Clamp( MinimumStrength, MaximumStrength );

		var twist = 0;
		twist += Input.Down( InputButton.Menu ) ? 1 : 0;
		twist += Input.Down( InputButton.Use ) ? -1 : 0;

		Twist += twist * Time.Delta * rate;
		Twist = Twist.Clamp( -MaximumTwist, MaximumTwist );
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
		BowlingBall.Velocity = Parent.Rotation.Forward * 35 * Strength;
		BowlingBall.AngularDirection = Parent.Rotation.Left * 3 * Twist;
		BowlingBall.Owner = this;

		EnableDrawing = false;
	}

	public void Reset()
	{
		EnableDrawing = true;

		Twist = DefaultTwist;
		Strength = DefaultStrength;
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
