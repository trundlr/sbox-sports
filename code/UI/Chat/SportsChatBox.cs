using Sports.PartySystem;

namespace Sports.UI;

[UseTemplate]
public partial class SportsChatBox : Panel
{

	/// <summary>
	/// Disable chat using ClientVar
	/// </summary>
	[ConVar.Client]
	public static bool ChatEnabled { get; set; } = true;

	public static SportsChatBox Instance;

	public Panel Canvas { get; protected set; }
	public ChatTextEntry Input { get; protected set; }
	public bool GlobalChat { get; protected set; } = true;

	public SportsChatBox()
	{
		Instance = this;
		Sports.Hooks.Chat.OnOpenChat += Open;
	}

	protected override void PostTemplateApplied()
	{
		base.PostTemplateApplied();
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;
	}

	public void SwitchChat()
	{
		if ( Local.Client.GetParty().IsValid() )
		{
			GlobalChat = !GlobalChat;
		}
		else
		{
			GlobalChat = true;
		}
		SetClass( "global", GlobalChat );
	}

	public void Open()
	{
		if ( !ChatEnabled )
			return;

		AddClass( "open" );

		if ( !Local.Client.GetParty().IsValid() )
		{
			GlobalChat = true;
			SetClass( "global", GlobalChat );
		}

		Input.Focus();
	}

	public void Close()
	{
		RemoveClass( "open" );
		Input.Blur();
	}

	public void Submit()
	{
		Close();

		var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg, GlobalChat );
	}

	protected string GetChatTypeIcon( string chatType )
	{
		return chatType switch
		{
			"Global" => "public",
			"Party" => "group",
			_ => "public"
		};
	}

	public void AddEntry( string name, string message, string avatar, string chatType, string lobbyState = null )
	{
		if ( !ChatEnabled )
			return;

		var e = Canvas.AddChild<SportsChatEntry>();
		e.ChatType.Text = $"{GetChatTypeIcon( chatType )}";
		e.AddClass( "type-" + chatType.ToLower() );
		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "no-name", string.IsNullOrEmpty( name ) );
		e.SetClass( "no-avatar", string.IsNullOrEmpty( avatar ) );

		if ( lobbyState == "ready" || lobbyState == "staging" )
		{
			e.SetClass( "is-lobby", true );
		}
	}

	[ConCmd.Client( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string avatar = null, string chatType = "Global", string lobbyState = null )
	{
		Instance?.AddEntry( name, message, avatar, chatType, lobbyState );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Debug( $"{name}: {message}" );
		}
	}

	[ConCmd.Client( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null )
	{
		Instance?.AddEntry( null, message, avatar, "System" );
	}

	[ConCmd.Server( "say" )]
	public static void Say( string message, bool global = true )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		// todo - reject more stuff
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		Log.Debug( $"{ConsoleSystem.Caller}: {message}" );

		if ( global )
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}", "Global" );
		else if ( ConsoleSystem.Caller.GetParty() is Party party )
			AddChatEntry( To.Multiple( party.Members ), ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}", "Party" );
	}
}


