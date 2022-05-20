namespace Sports;

public static class GamemodeExtensions
{
	/// <summary>
	/// Get the gamemode component of the client
	/// (Serverside Only) This will create a new component if it doesn't find one.
	/// </summary>
	public static GamemodeEntityComponent GetGamemodeComponent( this Client cl )
	{
		if ( Host.IsServer )
			return cl.Components.GetOrCreate<GamemodeEntityComponent>();
		return cl.Components.Get<GamemodeEntityComponent>();
	}

	/// <summary>
	/// Get the gamemode of the client
	/// (Serverside Only) This will create a new component if it doesn't find one.
	/// </summary>
	public static BaseGamemode GetGamemode( this Client cl )
	{
		return GetGamemodeComponent( cl )?.Gamemode;
	}
}
