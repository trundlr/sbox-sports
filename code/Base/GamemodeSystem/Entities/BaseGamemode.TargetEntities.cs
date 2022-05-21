namespace Sports;

public partial class BaseGamemode
{
	public List<IGamemodeEntity> Entities { get; set; }

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
