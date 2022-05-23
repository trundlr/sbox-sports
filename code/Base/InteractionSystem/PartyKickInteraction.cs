using Sports.PartySystem;

namespace Sports;

public class PartyKickInteraction : Interaction
{
	public override string ID => "kick_party";
	public override string NiceName => "Kick from party";

	public PartyKickInteraction( Entity interactableEntity )
	{
		Owner = interactableEntity;
	}

	public override bool CanResolve()
	{
		bool isHost = Owner.Client.GetParty()?.Host == Local.Client;
		return isHost;
	}

	protected override void OnServerResolve()
	{
		PartyManager.KickPlayer( Owner.Client.NetworkIdent );
	}
}
