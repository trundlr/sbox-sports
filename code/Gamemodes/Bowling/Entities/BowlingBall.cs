namespace Sports;

public class BowlingBall : ModelEntity
{
	private bool IsGrounded => GroundEntity != null;
	private float Gravity => 800f;
	private float Size => 18f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/bowling/bowlingball.vmdl" );
	}

	public override void Simulate( Client cl )
	{
		Move();
	}

	//
	// Predictable movement for bowling ball
	//
	private void Move()
	{
		var moveHelper = new MoveHelper( Position, Velocity )
		{
			GroundBounce = 0.1f,
			WallBounce = 0.25f
		};

		moveHelper.Trace = moveHelper.Trace.Size( Size );

		CheckGroundEntity();

		if ( !IsGrounded )
			moveHelper.Velocity += Vector3.Down * Gravity * Time.Delta;

		moveHelper.ApplyFriction( 0.1f, Time.Delta );
		moveHelper.TryMove( Time.Delta );

		Position = moveHelper.Position;
		Velocity = moveHelper.Velocity;

		Rotation *= Rotation.FromRoll( Velocity.Length );
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
