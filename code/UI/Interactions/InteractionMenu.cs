namespace Sports.UI;

public class InteractionMenu : Panel
{
	private List<Interaction> InteractionList { get; set; } = new();

	public Label NameLabel { get; set; }

	public InteractionMenu()
	{
		StyleSheet.Load( "/UI/Interactions/InteractionMenu.scss" );

		NameLabel = Add.Label( "placeholder", "name" );
	}

	public void SetEntity( Entity entity )
	{
		NameLabel.Text = entity.Client.Name;

		// Populate the InteractionMenu with the Entity's Interactions.
		if ( entity is IInteractable interactableEntity )
		{
			foreach ( Interaction interaction in interactableEntity.GetInteractions() )
				InteractionList.Add( interaction );

			CreatePanel();
		}

		// Delete this menu if the interaction list is empty.
		if ( InteractionList.Count == 0 )
			Delete();

	}

	public void CreatePanel()
	{
		foreach ( Interaction interaction in InteractionList )
			AddInteractionOption( interaction );
	}

	private Panel AddInteractionOption( Interaction interaction )
	{
		Label label = AddChild<Label>( "interaction-entry" );
		label.Text = interaction.InteractionName;
		label.AddEventListener( "onclick", () =>
		{
			interaction.Resolve();
			DeleteChildren();
			Delete();
		} );
		return label;
	}
}
