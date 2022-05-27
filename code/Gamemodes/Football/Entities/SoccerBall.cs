namespace Sports.Football;

public partial class SoccerBall : ModelEntity
{
	private float Radius => 25.9223f;
	private float Mass => 3f;

	private float frictionCoefficient = 0.5f;

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/football/football_ball.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Transmit = TransmitType.Always;

		Predictable = false;

		Tags.Add( "football" );
	}

}
