namespace Sports.Football;

public partial class SoccerBall
{
	protected Vector3 _velocity;
	public override Vector3 Velocity { get => _velocity; set => _velocity = value; }

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( IsServer )
		{
			using ( Prediction.Off() )
			{
				Move();
			}
		}
	}

	static Vector3 ProjectOntoPlane( Vector3 v, Vector3 normal, float overBounce = 1.0f )
	{
		float backoff = v.Dot( normal );

		if ( overBounce != 1.0f )
		{
			if ( backoff < 0 )
			{
				backoff *= overBounce;
			}
			else
			{
				backoff /= overBounce;
			}
		}

		return v - backoff * normal;
	}

	public virtual void Move()
	{
		var mover = new SoccerBallMover( Position, Velocity, "football", "football_ball_ignore" );
		mover.Trace = mover.Trace.Radius( Radius ).Ignore( this );
		mover.MaxStandableAngle = 50.0f;
		mover.GroundBounce = 0.9f;
		mover.WallBounce = 0.5f;

		var groundTrace = mover.TraceDirection( Vector3.Down * 0.1f );

		if ( groundTrace.Entity.IsValid() )
		{
			mover.GroundVelocity = groundTrace.Entity.Velocity;
		}

		// apply gravity
		mover.Velocity += Vector3.Down * 800 * Time.Delta;

		if ( groundTrace.Hit && groundTrace.Normal.Angle( Vector3.Up ) < 1.0f )
		{
			mover.Velocity = ProjectOntoPlane( mover.Velocity, groundTrace.Normal );
		}

		mover.TryMove( Time.Delta );
		mover.TryUnstuck();

		// apply friction based on our ground surface
		if ( groundTrace.Hit )
		{
			var friction = groundTrace.Surface.Friction;

			// apply more friction if the ball is close to stopping
			if ( mover.Velocity.Length < 1.0f )
				friction = 5.0f;
			else
				friction *= frictionCoefficient;

			mover.ApplyFriction( friction, Time.Delta );
		}
		else
		{
			// air drag
			mover.ApplyFriction( 0.5f, Time.Delta );
		}

		Position = mover.Position;
		BaseVelocity = mover.GroundVelocity;
		Velocity = mover.Velocity;

		// Rotate the ball
		if ( Velocity.LengthSquared > 0.0f )
		{
			var dir = Velocity.Normal;
			var axis = new Vector3( -dir.y, dir.x, 0.0f );
			var angle = (Velocity.Length * Time.Delta) / (10.0f * (float)Math.PI);
			Rotation = Rotation.FromAxis( axis, 180.0f * angle ) * Rotation;
		}

		if ( mover.Hit )
		{
			ImpactObject( (mover.HitEntity as ModelEntity).PhysicsBody, mover.HitPos, mover.HitVelocity );
			ImpactEffects( mover.HitPos, mover.HitNormal, mover.HitVelocity.Length );
		}
	}

	private void ImpactObject( PhysicsBody body, Vector3 hitpos, Vector3 velocity )
	{
		if ( body.IsValid() )
			body.ApplyForceAt( hitpos, velocity * 100.0f );
	}

	private void ImpactEffects( Vector3 pos, Vector3 normal, float speed )
	{
		DebugOverlay.Line( pos, pos + normal * speed * 0.01f, 10.0f );
		DebugOverlay.Sphere( pos, 1.0f, Color.Red, 10.0f );
	}

	[Event.Tick.Server]
	public void Tick()
	{
		DebugOverlay.Sphere( Position, 0.1f, Color.White, 10.0f );
	}
}
