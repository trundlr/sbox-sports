using Sandbox.UI.Construct;
using Sports.PartySystem;
using System;

namespace Sports.UI;
[UseTemplate]
public partial class SportsChatBox : Panel
{
	static SportsChatBox Current;

	public Panel Canvas { get; protected set; }
	public PartyChatTextEntry Input { get; protected set; }
	public bool GlobalChat { get; protected set; } = true;


	public SportsChatBox()
	{
		Current = this;
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
		if ( Local.Client.Components.Get<PartyComponent>()?.Party.IsValid() ?? false )
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
		AddClass( "open" );
		if ( !Local.Client.Components.Get<PartyComponent>()?.Party.IsValid() ?? true )
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

	public void AddEntry( string name, string message, string avatar, string chattype, string lobbyState = null )
	{
		var e = Canvas.AddChild<SportsChatEntry>();
		e.ChatType.Text = $"[{chattype}]";
		e.ChatType.AddClass( "chat-type-" + chattype.ToLower() );
		e.Message.Text = message;
		e.NameLabel.Text = name;
		e.Avatar.SetTexture( avatar );

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "noavatar", string.IsNullOrEmpty( avatar ) );

		if ( lobbyState == "ready" || lobbyState == "staging" )
		{
			e.SetClass( "is-lobby", true );
		}
	}


	[ClientCmd( "chat_add", CanBeCalledFromServer = true )]
	public static void AddChatEntry( string name, string message, string avatar = null, string chattype = "Global", string lobbyState = null )
	{
		Current?.AddEntry( name, message, avatar, chattype, lobbyState );

		// Only log clientside if we're not the listen server host
		if ( !Global.IsListenServer )
		{
			Log.Debug( $"{name}: {message}" );
		}
	}

	[ClientCmd( "chat_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message, string avatar = null )
	{
		Current?.AddEntry( null, message, avatar, "System" );
	}

	[ServerCmd( "say" )]
	public static void Say( string message, bool global = true )
	{
		Assert.NotNull( ConsoleSystem.Caller );

		// todo - reject more stuff
		if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;

		Log.Debug( $"{ConsoleSystem.Caller}: {message}" );
		if ( global )
			AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}", "Global" );
		else if ( ConsoleSystem.Caller.Components.Get<PartyComponent>() is PartyComponent comp && comp.Party.IsValid() )
			AddChatEntry( To.Multiple( comp.Party.Members ), ConsoleSystem.Caller.Name, message, $"avatar:{ConsoleSystem.Caller.PlayerId}", "Party" );
	}
}


