using Sports.PartySystem;

namespace Sports;

public partial class PartyInviteInteraction : Interaction
{
	public override string ID => "invite_party";
	public override string NiceName => "Invite to party";

	public PartyInviteInteraction( Entity interactableEntity )
	{
		Owner = interactableEntity;
	}

	public override bool CanResolve()
	{
		return !Owner.Client.IsSameParty( Local.Client );
	}

	protected override void OnServerResolve()
	{
		if ( Owner.Client.GetParty() == null )
			PartyManager.InvitePlayer( Owner.NetworkIdent );
	}
}
