namespace Sports;

public static class Debug
{
	[ConVar.Replicated( "debug" )]
	public static int Level { get; set; }

	public static bool Enabled => Level > 0;
}

public static class LoggerExtension
{
	public static void Debug( this Logger log, object obj )
	{
		if ( !Sports.Debug.Enabled ) return;

		log.Info( $"[{(Host.IsClient ? "CL" : "SV")}] {obj}" );
	}

	public static void Debug( this Logger log, object obj, int level )
	{
		if ( !(Sports.Debug.Level >= level) ) return;

		log.Info( $"[{(Host.IsClient ? "CL" : "SV")}] {obj}" );
	}
}
