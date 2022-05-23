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
		InteractionList.Clear();

		NameLabel.Text = entity.Client.Name;

		// Populate the InteractionMenu with the Entity's Interactions.
		if ( entity is IInteractable interactableEntity )
		{
			foreach ( Interaction interaction in interactableEntity.GetInteractions().Where( interaction => interaction.CanResolve() ) )
				InteractionList.Add( interaction );

			CreatePanel();
		}
	}

	public void CreatePanel()
	{
		foreach ( Interaction interaction in InteractionList )
			AddInteractionOption( interaction );
	}

	private void AddInteractionOption( Interaction interaction )
	{
		if ( !interaction.CanResolve() )
			return;

		Label label = AddChild<Label>( "interaction-entry" );
		label.Text = interaction.NiceName;

		label.AddEventListener( "onclick", () =>
		{
			interaction.ClientResolve();

			if ( interaction.ResolveOnServer )
				Interaction.TryServerResolve( interaction.Owner.NetworkIdent, interaction.ID );

			foreach ( var child in Children )
			{
				if ( child == NameLabel )
					continue;

				child.Delete();
			}

			InteractionList.Clear();

		} );
	}

	public override void Tick()
	{
		SetClass( "hide", InteractionList.Count == 0 );

		if ( InteractionList.Count > 0 )
			return;

		var player = Local.Pawn;
		var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 200 )
			.Ignore( player )
			.Run();

		if ( Debug.Enabled )
			DebugOverlay.TraceResult( tr );

		if ( tr.Hit && tr.Entity is Entity entity )
		{
			if ( Input.Pressed( InputButton.Use ) )
			{
				if ( entity is not IInteractable )
					return;

				SetEntity( entity );
			}

		}
	}
}
