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

		PartyCanvas.SetClass( "InParty", Party.IsValid() );

		if ( !Party.IsValid() )
			PartyCanvas.RemoveClass( "Control" );
	}

	[Event.BuildInput]
	private void BuildInput( InputBuilder builder )
	{
		PartyCanvas.SetClass( "IsHost", Local.Client == Party?.Host );
		PartyCanvas.SetClass( "Control", Input.Down( InputButton.Score ) );
	}

	public static void AcceptedInvite( int networkIdent )
	{
		Instance.InviteList.DeleteChildren( false );
		Party.AcceptInvite( networkIdent );
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
			member.AddClass( "LocalMember" );
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
