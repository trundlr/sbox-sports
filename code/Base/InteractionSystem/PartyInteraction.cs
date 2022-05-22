using Sports.PartySystem;

namespace Sports;

public partial class PartyInteraction : Interaction
{
	public override string InteractionName => "Add to party";

	public PartyInteraction( Entity interactableEntity )
	{
		Owner = interactableEntity;
	}

	protected override void OnResolve()
	{
		if ( Owner.Client.GetParty() == null )
			PartyManager.InvitePlayer( Owner.NetworkIdent );
	}
}
