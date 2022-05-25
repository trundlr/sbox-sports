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

		// create hull for triggers and such
		CollisionGroup = CollisionGroup.Player;
		AddCollisionLayer( CollisionLayer.Player );
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
		MoveType = MoveType.MOVETYPE_WALK;

		Tags.Add( "bowling_ball" );
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

		moveHelper.Trace = moveHelper.Trace.Size( Size ).Ignore( this ).WithoutTags( "bowling_owner", "bowling_pin" );

		CheckGroundEntity();

		if ( !IsGrounded )
			moveHelper.Velocity += Vector3.Down * Gravity * Time.Delta;

		moveHelper.ApplyFriction( 0.1f, Time.Delta );
		moveHelper.TryMove( Time.Delta );

		Position = moveHelper.Position;
		Velocity = moveHelper.Velocity;

		Rotation *= Rotation.FromRoll( Velocity.Length );

		if ( !Debug.Enabled )
			return;

		DebugOverlay.Text( $"grounded:{IsGrounded}\nspeed:{Velocity.Length}\nwall:{moveHelper.HitWall}", Position );
		DebugOverlay.Line( Position, Position + Velocity.Normal * 10 );
	}

	private void CheckGroundEntity()
	{
		var tr = Trace.Ray( Position, Position + Vector3.Down * 2 ).WorldOnly().Size( Size ).Run();

		if ( tr.Hit )
			GroundEntity = tr.Entity;
		else
			GroundEntity = null;
	}
}
