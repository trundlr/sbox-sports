namespace Sports.StateSystem;

public static class StateMachineExtensions
{
	public static StateMachine GetStateMachine( this Client cl )
	{
		return cl.GetGamemode()?.GetStateMachine<StateMachine>();
	}

	public static StateMachine GetStateMachine( this BaseGamemode gm )
	{
		return gm.GetStateMachine<StateMachine>();
	}

	public static T GetStateMachine<T>( this Client cl ) where T : StateMachine
	{
		return cl.GetGamemode()?.GetStateMachine<T>();
	}

	public static T GetStateMachine<T>( this BaseGamemode gm ) where T : StateMachine
	{
		if ( gm is IStateMachine<T> gamemode )
			return gamemode.StateMachine;
		if ( gm.StateMachine.IsValid() )
			return gm.StateMachine as T;
		return null;
	}
}
