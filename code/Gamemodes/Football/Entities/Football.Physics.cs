namespace Sports.Football;

public partial class Ball
{
	private readonly float Radius = 25.9223f;
	private readonly float FrictionStrength = 0.5f;

	protected Vector3 _Velocity;
	public override Vector3 Velocity { get => _Velocity; set => _Velocity = value; }

	public TimeSince SincePlayerKick { get; set; }
	public FootballPlayer KickingPlayer { get; set; }

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( IsServer )
		{
			using ( Prediction.Off() )
			{
				Move();
			}

			if ( SincePlayerKick > 0.5f )
			{
				KickingPlayer = null;
			}
		}
	}

	public void SetActivePlayer( FootballPlayer player )
	{
		KickingPlayer = player;
		SincePlayerKick = 0;
	}

	public virtual void Move()
	{
		var mover = new BallMover( Position, Velocity, "", "football", "football_ball_ignore" );
		mover.Trace = mover.Trace.Radius( Radius ).Ignore( this ).Ignore( KickingPlayer );
		mover.MaxStandableAngle = 50.0f;
		mover.GroundBounce = 0.9f;
		mover.WallBounce = 0.5f;

		var groundTrace = mover.TraceDirection( Vector3.Down * 0.1f );

		if ( groundTrace.Entity.IsValid() && groundTrace.Entity is not FootballPlayer )
		{
			mover.GroundVelocity = groundTrace.Entity.Velocity;
		}

		// apply gravity
		mover.Velocity += Vector3.Down * 800 * Time.Delta;

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
			else
				friction *= FrictionStrength;

			mover.ApplyFriction( friction, Time.Delta );
		}
		else
		{
			// air drag
			mover.ApplyFriction( 0.5f, Time.Delta );
		}

		Position = mover.Position;
		Velocity = mover.Velocity;

		// Rotate the ball
		if ( Velocity.LengthSquared > 0.0f )
		{
			var dir = Velocity.Normal;
			var axis = new Vector3( -dir.y, dir.x, 0.0f );
			var angle = (Velocity.Length * Time.Delta) / (Radius * (float)Math.PI);
			Rotation = Rotation.FromAxis( axis, 180.0f * angle ) * Rotation;
		}

		if ( mover.Hit && Debug.Enabled )
		{
			ImpactEffects( mover.HitPos, mover.HitNormal, mover.HitVelocity.Length );
		}
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
