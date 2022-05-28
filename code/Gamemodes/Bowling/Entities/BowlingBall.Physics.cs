namespace Sports;

public partial class BowlingBall
{
	protected Vector3 _Velocity;
	public override Vector3 Velocity { get => _Velocity; set => _Velocity = value; }
	/// <summary>
	/// Direction to deviate the ball's velocity towards. Can be used as a simple Angular Velocity spin.
	/// </summary>
	public Vector3 AngularDirection { get; set; }
	public Vector3 Gravity => Vector3.Down * 350.0f;

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

	public virtual void Move()
	{
		Host.AssertServer();

		var mover = new BallMover( Position, Velocity, "bowling_obstructor", "bowling_ball", "bowling_ball_ignore" );
		mover.Trace = mover.Trace.Radius( Radius ).Ignore( this );

		var groundTrace = mover.TraceDirection( Gravity.Normal * 0.5f );

		if ( groundTrace.Entity.IsValid() )
		{
			mover.GroundVelocity = groundTrace.Entity.Velocity;
		}

		// apply gravity
		mover.Velocity += Gravity * Time.Delta;

		// apply simple angular twist
		mover.Velocity += AngularDirection * Time.Delta * 0.5f;

		// we're on a straight surface, align our velocity so we don't vibrate along the surface because of clip bumps
		if ( groundTrace.Hit && groundTrace.Normal.Angle( Vector3.Up ) < 1.0f )
		{
			mover.ProjectOntoPlane( groundTrace.Normal );
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
			var angle = (Velocity.Length * Time.Delta) / (3.0f * (float)Math.PI);
			Rotation = Rotation.FromAxis( axis, 180.0f * angle ) * Rotation;
		}

		if ( mover.Hit )
		{
			// try to shove our hit object
			ImpactObject( (mover.HitEntity as ModelEntity).PhysicsBody, mover.HitPos, mover.HitNormal, mover.HitVelocity );

			// TODO: only send impact effect to clients participating in gamemode?
			ImpactEffects( mover.HitPos, mover.HitNormal, mover.HitVelocity.Length );
		}
	}

	private void ImpactObject( PhysicsBody body, Vector3 hitpos, Vector3 hitnormal, Vector3 velocity )
	{
		if ( !body.IsValid() )
			return;

		var vel = -hitnormal * velocity.Dot( -hitnormal );
		var force = Mass * (vel / Time.Delta);
		body.ApplyForceAt( hitpos, force );
	}

	[ClientRpc]
	private void ImpactEffects( Vector3 pos, Vector3 normal, float speed )
	{
		if ( !Debug.Enabled )
			return;

		DebugOverlay.Line( pos, pos + normal * speed * 0.01f, 10.0f );
		DebugOverlay.Sphere( pos, 1.0f, Color.Red, 10.0f );
	}

	[Event.Tick.Server]
	public void Tick()
	{
		if ( !Debug.Enabled )
			return;

		DebugOverlay.Sphere( Position, 0.1f, Color.White, 10.0f );
	}
}
