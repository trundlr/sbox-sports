namespace Sports;

public partial class BowlingPin : ModelEntity
{
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
		base.Spawn();

		Transmit = TransmitType.Always;
		SetModel( "models/bowling/bpin.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );

		MoveType = MoveType.Physics;
		CollisionGroup = CollisionGroup.Weapon;
		PhysicsEnabled = true;
		UsePhysicsCollision = true;

		Tags.Add( "bowling_pin" );
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
