namespace Sports.StateSystem;

public static class TurnManagerExtensions
{
	public static bool HasTurn( this Client cl )
	{
		return cl.GetGamemode()?.HasTurn( cl ) ?? false;
	}

	public static bool HasTurn( this BaseGamemode gm, Client cl )
	{
		return gm is ITurnStateMachine gamemode && gamemode.TurnStateMachine?.CurrentTurn == cl;
	}

	public static TurnStateMachine GetTurnStateMachine( this Client cl )
	{
		return cl.GetGamemode()?.GetTurnStateMachine<TurnStateMachine>();
	}

	public static TurnStateMachine GetTurnStateMachine( this BaseGamemode gm )
	{
		return gm.GetTurnStateMachine<TurnStateMachine>();
	}

	public static T GetTurnStateMachine<T>( this Client cl ) where T : TurnStateMachine
	{
		return cl.GetGamemode()?.GetTurnStateMachine<T>();
	}

	public static T GetTurnStateMachine<T>( this BaseGamemode gm ) where T : TurnStateMachine
	{
		return gm as T;
	}

	public static void EndTurn( this Client cl )
	{
		if ( !cl.HasTurn() )
			return;
		cl.GetTurnStateMachine()?.EndTurn();
	}
}
