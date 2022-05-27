namespace Sports;

public partial class BowlingBall : ModelEntity
{
	// 13 lbs = 5.89 kg
	// 4.2975 inch radius
	public static float Radius = 4.2975f;
	public static float Mass = 5.89f;

	static readonly Model BallModel = Model.Load( "models/bowling/bowlingball.vmdl" );

	public override void Spawn()
	{
		base.Spawn();

		Model = BallModel;

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		CollisionGroup = CollisionGroup.Debris;
		EnableTraceAndQueries = false;

		Transmit = TransmitType.Always;

		Predictable = false;

		Tags.Add( "bowling_ball" );
	}
}
