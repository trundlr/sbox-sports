using Sports.StateSystem;

namespace Sports.Football.States;

public class FootballWarmupState : BaseState<FootballStateMachine>
{
	public override void OnEnter()
	{
		base.OnEnter();
		StateMachine.GameActive = false;
	}
}
