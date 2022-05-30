namespace Sports.Football;

public partial class SoccerBall : ModelEntity
{

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/football/football_ball.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Transmit = TransmitType.Always;

		Predictable = true;

		Tags.Add( "football" );
	}

}
