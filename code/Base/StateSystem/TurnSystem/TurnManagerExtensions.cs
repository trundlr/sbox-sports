namespace Sports.StateSystem;

public static class TurnManagerExtensions
{
	public static bool HasTurn( this Client cl )
	{
		return cl.GetGamemode()?.HasTurn( cl ) ?? false;
	}

	public static bool HasTurn( this BaseGamemode gm, Client cl )
	{
		return gm?.GetStateMachine<TurnStateMachine>()?.CurrentTurn == cl;
	}

	private static TurnStateMachine GetTurnStateMachine( this Client cl )
	{
		return cl.GetGamemode()?.GetStateMachine<TurnStateMachine>();
	}

	public static void EndTurn( this Client cl )
	{
		if ( !cl.HasTurn() )
			return;
		cl.GetTurnStateMachine().EndTurn();
	}
}
