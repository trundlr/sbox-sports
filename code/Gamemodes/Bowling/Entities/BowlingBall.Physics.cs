﻿namespace Sports;

public partial class BowlingBall
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

	public virtual void Move()
	{
		var mover = new BallMover( Position, Velocity, "bowling_ball", "bowling_ball_ignore" );
		mover.Trace = mover.Trace.Radius( Radius ).Ignore( this );

		var groundTrace = mover.TraceDirection( Vector3.Down * 0.5f );

		if ( groundTrace.Entity.IsValid() )
		{
			mover.GroundVelocity = groundTrace.Entity.Velocity;
		}

		// apply gravity
		mover.Velocity += Vector3.Down * 800 * Time.Delta;

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
			ImpactObject( (mover.HitEntity as ModelEntity).PhysicsBody, mover.HitPos, mover.HitVelocity );
			ImpactEffects( mover.HitPos, mover.HitNormal, mover.HitVelocity.Length );
		}
	}

	private void ImpactObject( PhysicsBody body, Vector3 hitpos, Vector3 velocity )
	{
		if ( !body.IsValid() )
			return;

		var force = Mass * (Velocity / Time.Delta);
		body.ApplyForceAt( hitpos, force );
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
