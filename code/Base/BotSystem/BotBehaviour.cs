namespace Sports.BotSystem;

public class BaseBotBehaviour
{
	public SportsBot Bot { get; set; }

	public NavSteer Steer;
	public Draw Draw => Draw.Once;

	public bool Attack1 { get; set; }
	public bool Attack2 { get; set; }
	public Angles ViewAngles { get; set; }
	public Vector3 InputDirection { get; set; }

	public Vector3 InputVelocity { get; protected set; }



	public virtual void Tick()
	{
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
