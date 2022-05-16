namespace Sports;

public partial class GamemodeEntityComponent : EntityComponent
{
	public static GamemodeEntityComponent GetOrCreate( Client cl )
	{
		return cl?.Components.GetOrCreate<GamemodeEntityComponent>();
	}

	[Net]
	public BaseGamemode Gamemode { get; set; }
}
