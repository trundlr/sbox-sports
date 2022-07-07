using Sports.PartySystem;

namespace Sports;

public partial class TestInteraction : Interaction
{
	public override string ID => "test";
	public override string NiceName => $"Say hi to {Owner.Client?.Name ?? "Unknown player"}";

	public TestInteraction( Entity interactableEntity )
	{
		Owner = interactableEntity;
	}

	public override bool CanResolve()
	{
		return true;
	}

	public override bool ResolveOnServer => false;

	protected override void OnClientResolve()
	{
		UI.SportsChatBox.Say( $"Hey, {Owner.Client?.Name ?? "Unknown player"}" );
	}
}
