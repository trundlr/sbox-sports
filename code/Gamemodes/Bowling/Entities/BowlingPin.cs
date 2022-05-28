namespace Sports;

public partial class BowlingPin : BasePhysics
{
	static readonly Model PinModel = Model.Load( "models/bowling/bpin.vmdl" );

	[Net]
	public int Index { get; set; }
	[Net]
	public Vector3 InitialPosition { get; set; }
	[Net]
	public BowlingPinGroup PinGroup { get; set; }
	[Net]
	public bool Tipped { get; set; }

	public override void Spawn()
	{
		Model = PinModel;

		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		base.Spawn();

		Transmit = TransmitType.Always;

		Tags.Add( "bowling_obstructor", "bowling_pin" );
	}

	[Event.Tick.Server]
	public void Tick()
	{
		// count as tipped if we're either at an angle or too far away from our initial position
		if ( !Tipped && (Rotation.Up.Dot( Vector3.Up ) <= 0.75f || Position.Distance( InitialPosition ) >= 10) )
		{
			Tipped = true;
		}
	}
}
