namespace Sports.UI;

[Library]
public class NameplateRootPanel : Panel
{
	Dictionary<Entity, PlayerNameplate> Active = new();

	public override void Tick()
	{
		var deleteList = new List<Entity>();
		deleteList.AddRange( Active.Keys );

		// We can definitely do better than this, but for now it's okay.
		foreach ( var entity in Entity.All.OfType<BasePlayer>().Where( x => x.LifeState == LifeState.Alive && !x.IsLocalPawn ).OrderBy( x => Vector3.DistanceBetween( x.EyePosition, CurrentView.Position ) ) )
		{
			if ( UpdateNameTag( entity ) )
				deleteList.Remove( entity );
		}

		foreach ( var player in deleteList )
		{
			Active[player].Delete();
			Active.Remove( player );
		}
	}

	public bool UpdateNameTag( BasePlayer entity )
	{
		if ( !entity.Client.IsValid() )
			return false;

		if ( !Active.TryGetValue( entity, out var tag ) )
		{
			tag = new PlayerNameplate( entity );
			Active[entity] = tag;
		}

		tag.UpdateFromPlayer( entity );

		return true;
	}
}
