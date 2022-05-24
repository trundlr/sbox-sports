namespace Sports.Football;

public class Ball : ModelEntity
{
	private bool IsGrounded => GroundEntity != null;
	private float Gravity => 800f;
	private float Size => 50f;

	public override void Spawn()
	{
		base.Spawn();
		SetModel( "models/football/football_ball.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );

		Tags.Add( "ball" );

		Predictable = true;
	}

	public override void Simulate( Client cl )
	{
		Move();
	}

	//
	// Predictable movement for Football ball
	//
	private void Move()
	{
		var moveHelper = new MoveHelper( Position, Velocity )
		{
			GroundBounce = 0.5f,
			WallBounce = 0.25f
		};

		moveHelper.Trace = moveHelper.Trace.Size( Size ).Ignore( this );

		CheckGroundEntity();

		if ( !IsGrounded )
			moveHelper.Velocity += Vector3.Down * Gravity * Time.Delta;

		moveHelper.ApplyFriction( 0.2f, Time.Delta );
		moveHelper.TryMove( Time.Delta );

		Position = moveHelper.Position;
		Velocity = moveHelper.Velocity;

		Rotation *= Rotation.FromRoll( Velocity.Length / 100 );
	}

	private void CheckGroundEntity()
	{
		var tr = Trace.Ray( Position, Position ).WorldOnly().Size( Size ).Run();
		if ( tr.Hit )
			GroundEntity = tr.Entity;
		else
			GroundEntity = null;
	}
}
