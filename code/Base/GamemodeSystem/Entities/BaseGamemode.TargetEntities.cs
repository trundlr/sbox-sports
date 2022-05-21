namespace Sports;

public partial class BaseGamemode
{
	public List<GamemodePointEntity> Entities { get; protected set; } = new();

	public IEnumerable<Entity> GetEntitiesWithTag( string tag )
	{
		return Entities.Where( x => x.Tags.Has( tag ) );
	}
	public IEnumerable<T> GetEntitiesOfType<T>( string tag = null )
	{
		if ( tag is null )
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
		var ents = All.OfType<GamemodePointEntity>();

		Log.Info( $"[{Name}] Finding gamemode entities, count: {ents.Count()}" );

		foreach ( var gamemodeEntity in ents )
		{
			if ( gamemodeEntity.GamemodeId.ToLower() == Name.ToLower() )
			{
				Log.Info( $"	> Matched GamemodePointEntity for {Name}" );

				Entities.Add( gamemodeEntity );
				gamemodeEntity.Gamemode = this;
				gamemodeEntity.Parent = this;
			}
		}
	}
}
