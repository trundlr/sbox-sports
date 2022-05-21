using Sports.PartySystem;

namespace Sports;

public partial class PartyAddInteraction : Interaction
{
	public override string InteractionName => "Add to party";

	public PartyAddInteraction( Entity interactableEntity )
	{
		Owner = interactableEntity;
	}

	protected override void OnResolve()
	{
		if ( Owner.Client.GetParty() == null )
			PartyManager.InvitePlayer( Owner.NetworkIdent );
	}
}
