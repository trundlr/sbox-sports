namespace Sports;

public partial class BaseGamemode
{
	/// <summary>
	/// A ReadOnly Collection of all gamemode entities associated to this gamemode.
	/// </summary>
	public IList<Entity> Entities => _Entities.AsReadOnly();
	private List<Entity> _Entities { get; set; } = new();

	/// <summary>
	/// Utility function to get a collection of all gamemode entities with a specified tag.
	/// </summary>
	/// <param name="tag">The tag to filter for.</param>
	/// <returns>A collection of gamemode entities associated to this gamemode with the specified tag.</returns>
	public IEnumerable<Entity> GetEntitiesWithTag( string tag )
	{
		return Entities.Where( x => x.Tags.Has( tag ) );
	}

	/// <summary>
	/// Utility function to get a collection of all gamemode entities with a specified tag and of a specified type.
	/// </summary>
	/// <typeparam name="T">The type of Entities.</typeparam>
	/// <param name="tag">The tag to filter for.</param>
	/// <returns>A collection of gamemode entities of type T associated to this gamemode with the specified tag, if any.</returns>
	public IEnumerable<T> GetEntitiesOfType<T>( string tag = null )
	{
		if ( tag is null )
		{
			return Entities.OfType<T>();
		}
		else
		{
			return GetEntitiesWithTag( tag ).OfType<T>();
		}
	}

	[Event.Entity.PostSpawn]
	private void OnPostSpawn()
	{
		var ents = All.OfType<IGamemodeEntity>();

		Log.Debug( $"[{Name}] Finding gamemode entities, total: {ents.Count()}" );

		foreach ( var ent in ents )
		{
			// this is a bit whack, but the interface is forcing our hand
			if ( ent is not Entity gamemodeEnt )
				continue;

			if ( ent.GamemodeName.ToLower() == Name.ToLower() )
			{
				Log.Debug( $"	> Matched IGamemodeEntity for {Name}" );

				_Entities.Add( gamemodeEnt );
				ent.Gamemode = this;
				gamemodeEnt.Parent = this;
			}
		}

		Log.Debug( $"	> Successfully matched {_Entities.Count} Entities" );
	}
}
