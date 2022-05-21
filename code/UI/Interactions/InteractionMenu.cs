namespace Sports.UI;

public class InteractionMenu : Panel
{
	private Entity Entity { get; set; }
	private List<Interaction> InteractionList { get; set; } = new();

	public Label NameLabel { get; set; }

	public InteractionMenu()
	{
		StyleSheet.Load( "/UI/Interactions/InteractionMenu.scss" );

		NameLabel = Add.Label( "placeholder", "name" );
	}

	public void SetEntity( Entity entity )
	{
		Entity = entity;
		NameLabel.Text = entity.Client.Name;

		foreach ( Interaction interaction in (entity as BasePlayer).GetInteractions() )
		{
			InteractionList.Add( interaction );
		}
	}

	public void CreatePanel()
	{
		foreach ( Interaction interaction in InteractionList )
		{
			AddInteractionOption( interaction );
		}
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
