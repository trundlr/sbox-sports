namespace Sports;

public enum LeaveReason
{
	Disconnect,
	Leave,
	Kick
}

public static class LeaveReasonExtensions
{
	public static LeaveReason ToLeaveReason( this NetworkDisconnectionReason reason )
	{
		return reason switch
		{
			_ => LeaveReason.Disconnect
		};
	}
}
