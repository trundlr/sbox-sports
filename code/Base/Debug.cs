namespace Sports;

public static class Debug
{
	public static int Level { get; set; } = 10;

	// @TODO: revert when ConVar.Replicated is fixed
	public static bool Enabled => true;
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
