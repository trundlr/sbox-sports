using Sports.PartySystem;

namespace Sports.UI;

[UseTemplate]
public class PartyMember : Panel
{

	public Client Client { get; internal set; }
	public Label Name { get; internal set; }
	public Image Avatar { get; internal set; }

	public void Kick()
	{
		PartyManager.KickPlayer( Client.NetworkIdent );
	}

	public void Leave()
	{
		PartyManager.LeaveParty();
	}

}
