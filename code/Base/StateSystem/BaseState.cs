namespace Sports.StateSystem;

public class BaseState : BaseNetworkable
{
	public StateMachine StateMachine { get; set; } 

	public virtual void CheckSwitchState() { }

	public virtual void OnEnter() { }

	public virtual void OnExit() { }

	public virtual void OnTick() { }

}
