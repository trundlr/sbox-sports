using Sports.PartySystem;

namespace Sports.UI;

[UseTemplate]
public partial class PartyLobby : Panel
{
	public static PartyLobby Instance;

	public PartyLobby()
	{
		Instance = this;
	}

	public Panel PartyCanvas { get; protected set; }
	public Panel InviteList { get; protected set; }
	public Party Party => Local.Client?.GetParty();

	public override void Tick()
	{
		base.Tick();

		SetClass( "in-party", Party.IsValid() );

		if ( !Party.IsValid() )
			PartyCanvas.RemoveClass( "control" );
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder builder )
	{
		PartyCanvas.SetClass( "is-host", Local.Client == Party?.Host );
		PartyCanvas.SetClass( "control", Input.Down( InputButton.Score ) );
	}

	public static void AcceptedInvite( int networkIdent )
	{
		Instance.InviteList.DeleteChildren( false );
		PartyManager.AcceptInvite( networkIdent );
	}

	public static void AddPartyMember( Client client )
	{
		if ( Instance is null )
			return;

		var member = Instance.PartyCanvas.AddChild<PartyMember>();
		member.Client = client;
		member.Avatar.SetTexture( $"avatar:{client.PlayerId}" );
		member.Name.SetText( client.Name );

		if ( client == Local.Client )
			member.AddClass( "local-member" );
	}

	public static void OnPartyChanged()
	{
		if ( Instance is null )
			return;

		Instance.PartyCanvas?.DeleteChildren();

		if ( Instance.Party?.Members is null )
			return;

		foreach ( var member in Instance.Party?.Members )
		{
			AddPartyMember( member );
		}
	}

	protected override void PostTemplateApplied()
	{
		base.PostTemplateApplied();
		OnPartyChanged();
	}

	public static void OnInviteReceived( Client cl )
	{
		if ( Instance == null || Instance.InviteList.Children.Any( e => e is PartyInvite partyInvite && partyInvite.Client == cl ) )
			return;

		var invite = Instance.InviteList.AddChild<PartyInvite>();
		invite.Client = cl;
		invite.InviteText.Text = $"{cl.Name} invited you to their party!";
		invite.AddClass( "open" );
	}
}
