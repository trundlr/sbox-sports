namespace Sports;

public static class GamemodeExtensions
{
	/// <summary>
	/// Get the gamemode component of the client
	/// </summary>
	public static GamemodeEntityComponent GetGamemodeComponent( this Client cl )
	{
		if ( Host.IsServer )
			return cl.Components.GetOrCreate<GamemodeEntityComponent>();
		return cl.Components.Get<GamemodeEntityComponent>();
	}

	/// <summary>
	/// Get the gamemode of the client
	/// </summary>
	public static BaseGamemode GetGamemode( this Client cl )
	{
		return GetGamemodeComponent( cl )?.Gamemode;
	}
}
