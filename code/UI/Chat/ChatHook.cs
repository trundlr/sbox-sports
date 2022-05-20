namespace Sports.Hooks;

public static partial class Chat
{
	public static event Action OnOpenChat;

	[ConCmd.Client( "openchat" )]
	internal static void MessageMode()
	{
		OnOpenChat?.Invoke();
	}

}
