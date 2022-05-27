namespace Sports;

public struct BallMover
{
	public Vector3 Position;
	public Vector3 Velocity;

	public bool Hit;
	public Entity HitEntity;
	public Vector3 HitPos;
	public Vector3 HitNormal;
	public Vector3 HitVelocity;

	public Vector3 GroundVelocity;
	public float GroundBounce;
	public float WallBounce;
	public float MaxStandableAngle;
	public Trace Trace;

	public BallMover( Vector3 position, Vector3 velocity, params string[] ignoretags ) : this()
	{
		Velocity = velocity;
		Position = position;
		GroundVelocity = Vector3.Zero;
		GroundBounce = 0.0f;
		WallBounce = 0.0f;
		MaxStandableAngle = 45.0f;

		// Hit everything but other balls and things marked to specifically be ignored by the ball
		Trace = Trace.Ray( 0, 0 )
			.WorldAndEntities()
			.HitLayer( CollisionLayer.Solid, true )
			.HitLayer( CollisionLayer.PLAYER_CLIP, true )
			.HitLayer( CollisionLayer.GRATE, true )
			.WithoutTags( ignoretags );
	}

	public TraceResult TraceFromTo( Vector3 start, Vector3 end )
	{
		return Trace.FromTo( start, end ).Run();
	}

	public TraceResult TraceDirection( Vector3 down )
	{
		return TraceFromTo( Position, Position + down );
	}

	public float TryMove( float timestep )
	{
		var timeLeft = timestep;
		float travelFraction = 0;

		Hit = false;
		HitEntity = null;
		HitPos = Vector3.Zero;
		HitNormal = Vector3.Zero;
		HitVelocity = Vector3.Zero;

		Position += GroundVelocity * timestep;

		using var moveplanes = new VelocityClipPlanes( Velocity );

		for ( int bump = 0; bump < moveplanes.Max; bump++ )
		{
			if ( Velocity.Length.AlmostEqual( 0.0f ) )
				break;

			var pm = TraceFromTo( Position, Position + Velocity * timeLeft );

			if ( pm.StartedSolid )
			{
				Position += pm.Normal * 0.01f;

				continue;
			}

			travelFraction += pm.Fraction;

			if ( pm.Fraction > 0.0f )
			{
				Position = pm.EndPosition + pm.Normal * 0.01f;

				moveplanes.StartBump( Velocity );
			}

			if ( !Hit && !pm.StartedSolid && pm.Hit )
			{
				Hit = true;
				HitEntity = pm.Entity;
				HitPos = pm.HitPosition;
				HitNormal = pm.Normal;
				HitVelocity = Velocity;
			}

			timeLeft -= timeLeft * pm.Fraction;

			if ( !moveplanes.TryAdd( pm.Normal, ref Velocity, IsFloor( pm ) ? GroundBounce : WallBounce ) )
				break;
		}

		if ( travelFraction == 0 )
			Velocity = 0;

		return travelFraction;
	}

	public bool IsFloor( TraceResult tr )
	{
		if ( !tr.Hit ) return false;
		return tr.Normal.Angle( Vector3.Up ) < MaxStandableAngle;
	}

	public void ApplyFriction( float frictionAmount, float delta )
	{
		float StopSpeed = 100.0f;

		var speed = Velocity.Length;
		if ( speed < 0.1f )
		{
			Velocity = 0;
			return;
		}

		// Bleed off some speed, but if we have less than the bleed
		//  threshold, bleed the threshold amount.
		float control = (speed < StopSpeed) ? StopSpeed : speed;

		// Add the amount to the drop amount.
		var drop = control * delta * frictionAmount;

		// scale the velocity
		float newspeed = speed - drop;
		if ( newspeed < 0 ) newspeed = 0;
		if ( newspeed == speed ) return;

		newspeed /= speed;
		Velocity *= newspeed;
	}

	public void TryUnstuck()
	{
		var tr = TraceFromTo( Position, Position );
		if ( !tr.StartedSolid ) return;

		Position += tr.Normal * 1.0f;
		Velocity += tr.Normal * 50.0f;
	}
}
