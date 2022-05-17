
using Sports.UI;

namespace Sports;

public class SportsHud : RootPanel
{
	public SportsHud()
	{
		AddChild<GamemodeHudPanel>();
		AddChild<SportsChatBox>();
		AddChild<PartyLobby>();
	}

	public override void OnDeleted()
	{
		DeleteChildren( true );

		base.OnDeleted();
	}
}
