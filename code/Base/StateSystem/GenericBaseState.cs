namespace Sports.StateSystem;

public class BaseState<T> : BaseState where T : StateMachine
{
	public new T StateMachine
	{
		get
		{
			return base.StateMachine as T;
		}
		internal set
		{
			base.StateMachine = value;
		}
	}
}
