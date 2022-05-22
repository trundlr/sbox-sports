using Sports.PartySystem;

namespace Sports;

public partial class PartyInteraction : Interaction
{
	public override string ID => "invite_party";
	public override string NiceName => "Add to party";

	public PartyInteraction( Entity interactableEntity )
	{
		Owner = interactableEntity;
	}

	protected override void OnServerResolve()
	{
		if ( Owner.Client.GetParty() == null )
			PartyManager.InvitePlayer( Owner.NetworkIdent );
	}
}
