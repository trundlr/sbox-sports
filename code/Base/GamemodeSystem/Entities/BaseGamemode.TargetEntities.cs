namespace Sports;

public partial class BaseGamemode
{
	public List<Entity> Entities { get; protected set; }

	public IEnumerable<Entity> GetEntitiesWithTag( string tag )
	{
		return Entities.Where( x => x.Tags.Has( tag ) );
	}
	public IEnumerable<T> GetEntitiesOfType<T>( string tag = null )
	{
		if ( tag is not null )
		{
			return Entities.OfType<T>();
		}
		else
		{
			return GetEntitiesWithTag( tag )
				.OfType<T>();
		}
	}

	[Event.Entity.PostSpawn]
	protected virtual void GatherTargetEntities()
	{
		var ents = FindAllByName( Name );

		foreach ( var entity in ents )
		{
			if ( entity == this )
				continue;

			Log.Debug( $"Found gamemode entity for {Name} - {entity.GetType()}" );

			if ( entity is IGamemodeEntity gamemodeEntity )
			{
				Entities.Add( gamemodeEntity );
				gamemodeEntity.Gamemode = this;
				entity.Parent = this;
			}
		}
	}
}
