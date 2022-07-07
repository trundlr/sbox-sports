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

	}

	protected bool ShouldHideArrows()
	{
		if ( InteractionList.Count == 1 )
		{
			return true;
		}

		return false;
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
		UpdateArrows();

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

		var label = Options.AddChild<InteractionEntry>( "interaction-entry" );
		label.Name.Text = interaction.NiceName;
		label.BindClass( "active-interaction", () => index == CurrentInteractionIndex );

		UpdateArrows();
	}

	protected void Use()
	{
		if ( InteractionList.Count == 0 )
			return;

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

		UpdateArrows();
	}

	protected void UpdateArrows()
	{
		UpArrow.SetClass( "active", ShouldShowArrow( false ) );
		DownArrow.SetClass( "active", ShouldShowArrow( true ) );

		UpArrow.SetClass( "hide", ShouldHideArrows() );
		DownArrow.SetClass( "hide", ShouldHideArrows() );
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
			CurrentEntity = null;
			Clear();

			return;
		}

		if ( input.MouseWheel != 0f )
			DoMouseWheelInput( input.MouseWheel );

		if ( input.Pressed( InputButton.Use ) )
			Use();
	}
}
