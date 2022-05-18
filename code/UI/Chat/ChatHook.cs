namespace Sports.Hooks;

public static partial class Chat
{
	public static event Action OnOpenChat;

	[ClientCmd( "openchat" )]
	internal static void MessageMode()
	{
		OnOpenChat?.Invoke();
	}

}
