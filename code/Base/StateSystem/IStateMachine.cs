namespace Sports.StateSystem;

public interface IStateMachine<T> where T : StateMachine
{
	T StateMachine { get; set; }
}
