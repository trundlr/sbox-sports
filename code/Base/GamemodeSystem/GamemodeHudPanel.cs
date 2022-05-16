namespace Sports;

public partial class GamemodeHudPanel : Panel
{
	public Panel Panel { get; set; }

	[Events.Client.LocalGamemodeChanged]
	public void MinigameChanged( BaseGamemode gamemode )
	{
		if ( !gamemode.IsValid() )
		{
			Panel?.Delete( true );
			Panel = null;

			return;
		}

		Panel = gamemode.CreateHud();
		Panel.Parent = this;
	}
}
