using Sports.StateSystem;

namespace Sports.Football.States;

public class FBWarmupState : BaseState<FBStateMachine>
{
	public override void OnEnter()
	{
		base.OnEnter();
		StateMachine.GameActive = false;
	}
}
