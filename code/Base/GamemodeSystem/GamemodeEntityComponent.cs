namespace Sports;

public partial class GamemodeEntityComponent : EntityComponent
{
	public static GamemodeEntityComponent GetOrCreate( Client cl )
	{
		return cl?.Components.GetOrCreate<GamemodeEntityComponent>();
	}

	[Net, Change( nameof( OnGamemodeChanged ) )]
	public BaseGamemode Gamemode { get; set; }

	public void OnGamemodeChanged( BaseGamemode _, BaseGamemode newGamemode )
	{
		if ( Entity is Client cl )
		{
			if ( cl == Local.Client )
			{
				var eventName = cl == Local.Client ? Events.Client.LocalGamemodeChanged : Events.Client.ClientGamemodeChanged;
				Event.Run( eventName, newGamemode );
			}
		}
	}
}
