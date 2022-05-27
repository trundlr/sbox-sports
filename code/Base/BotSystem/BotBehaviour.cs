namespace Sports.BotSystem;

public class BaseBotBehaviour
{
	public SportsBot Bot { get; set; }

	public NavSteer Steer;
	public Draw Draw => Draw.Once;

	#region Randomized Variables
	private float turnSpeed = Rand.Float( 10f, 25f ); // lower is slower

	#endregion

	#region Inputs
	public bool Attack1 { get; set; }
	public bool Attack2 { get; set; }
	public Angles ViewAngles { get; set; }
	public Vector3 InputDirection { get; set; }
	#endregion

	public Entity CurrentTarget { get; protected set; }
	public Entity CurrentPlayer { get; protected set; }
	public Entity CurrentWeapon { get; protected set; }
	public Vector3 InputVelocity { get; protected set; }

	#region Static Variables
	public float UpdateInterval => 1.0f;
	public float SearchRadius => 400.0f;
	public float MinWanderRadius => 1000;
	public float MaxWanderRadius => 10000;
	public float PlayerOrbitDistance => 200;
	#endregion

	private TimeSince timeSinceUpdate;
	private Vector3 lastPos;

	public virtual void Tick()
	{
		if ( Debug.Bots )
		{
			DebugOverlay.Sphere( Bot.Client.Pawn.Position, SearchRadius, Color.Magenta );
			DebugOverlay.Text( $"{Bot.GetType().Name}\nFake Client Name: {Bot.Client.Name}\nCurrent Target: {(CurrentTarget != null ? CurrentTarget : "null")}", Bot.Client.Pawn.Position, CurrentTarget != null ? Color.Yellow : Color.White, 0, 1000 );
		}
	}

	/// <summary>
	/// Decide inputs
	/// </summary>
	public virtual void SetInputs()
	{
	}

	/// <summary>
	/// Decide where we want to go based on our target
	/// </summary>
	/// <param name="target"></param>
	/// <returns></returns>
	public virtual Vector3 EvaulatePositon( Entity target )
	{
		return 0;
	}

	/// <summary>
	/// Choose what the bot should move to; the main decision making process. Override this for different gamemodes ande write your own logic
	/// </summary>
	/// <returns></returns>
	public virtual Entity EvaulateTarget()
	{
		return null;
	}

	/// <summary>
	/// Finds the nearest entity within a sphere radius
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="position">Position of the sphere</param>
	/// <param name="radius">Radius of the sphere</param>
	/// <param name="ignore">Entities to ignore in the search</param>
	public static T GetClosestEntityInSphere<T>( Vector3 position, float radius, params Entity[] ignore ) where T : Entity
	{
		List<Entity> ents = Entity.FindInSphere( position, radius ).Where( x => x is T && !ignore.Contains( x ) ).ToList();
		Entity closestEnt = null;

		float smallestDist = 999999;
		foreach ( var ent in ents )
		{
			var dist = Vector3.DistanceBetween( position, ent.Position );
			if ( dist < smallestDist )
			{
				smallestDist = dist;
				closestEnt = ent;
			}
		}

		return closestEnt as T;
	}
}
