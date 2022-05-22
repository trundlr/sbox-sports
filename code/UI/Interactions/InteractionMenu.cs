namespace Sports.UI;

[UseTemplate]
public class InteractionMenu : Panel
{
	private List<Interaction> InteractionList { get; set; } = new();
	public Entity CurrentEntity { get; set; }

	// @ref
	public Panel Options { get; set; }
	// @ref
	public Panel UpArrow { get; set; }
	// @ref
	public Panel DownArrow { get; set; }

	public int CurrentInteractionIndex = 0;

	public InteractionMenu()
	{
		UpArrow.BindClass( "active", () => ShouldShowArrow( false ) );
		DownArrow.BindClass( "active", () => ShouldShowArrow( true ) );
	}

	protected bool ShouldShowArrow( bool ascending )
	{
		if ( ascending )
		{
			return CurrentInteractionIndex + 1 < InteractionList.Count;
		}
		else
		{
			return CurrentInteractionIndex - 1 >= 0;
		}
	}

	public void SetEntity( Entity entity )
	{
		if ( CurrentEntity == entity ) return;

		CurrentEntity = entity;
		CurrentInteractionIndex = 0;
		InteractionList.Clear();

		// Populate the InteractionMenu with the Entity's Interactions.
		if ( entity is IInteractable interactableEntity )
		{
			foreach ( Interaction interaction in interactableEntity.GetInteractions().Where( interaction => interaction.CanResolve() ) )
				InteractionList.Add( interaction );

			CreatePanel();
		}
	}

	protected void Clear()
	{
		CurrentEntity = null;
		InteractionList.Clear();
		Options.DeleteChildren( true );
	}

	public void CreatePanel()
	{
		int i = 0;
		foreach ( Interaction interaction in InteractionList )
		{
			AddInteractionOption( interaction, i );
			i++;
		}
	}

	private void AddInteractionOption( Interaction interaction, int index )
	{
		if ( !interaction.CanResolve() )
			return;

		Label label = Options.AddChild<Label>( "interaction-entry" );
		label.Text = interaction.NiceName;
		label.BindClass( "active-interaction", () => index == CurrentInteractionIndex );
	}

	protected void Use()
	{
		var interaction = InteractionList[CurrentInteractionIndex];

		interaction.ClientResolve();

		if ( interaction.ResolveOnServer )
			Interaction.TryServerResolve( interaction.Owner.NetworkIdent, interaction.ID );

		Clear();
	}

	protected void DoMouseWheelInput( int delta )
	{
		var isAscending = delta == 1;
		var length = InteractionList.Count();

		CurrentInteractionIndex += isAscending ? -1 : 1;
		CurrentInteractionIndex = (CurrentInteractionIndex + length) % length;
	}

	[Event.BuildInput]
	protected void BuildInput( InputBuilder input )
	{
		SetClass( "hide", InteractionList.Count == 0 );

		var player = Local.Pawn;
		var tr = Trace.Ray( player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 200 )
			.Ignore( player )
			.Run();

		if ( tr.Hit && tr.Entity is Entity entity )
		{
			if ( entity is not IInteractable )
				return;

			SetEntity( entity );
		}
		else
		{
			Clear();
			return;
		}

		if ( input.MouseWheel != 0f )
			DoMouseWheelInput( input.MouseWheel );

		if ( input.Pressed( InputButton.Use ) )
			Use();
	}
}
