namespace Sports.StateSystem;

[Library]
[PredictStates( nameof( WaitState ) )]
public class TurnState : BaseState<TurnStateMachine>
{
	public override void CheckSwitchState()
	{
		if ( StateMachine.TurnFinished )
		{
			StateMachine.TurnFinished = false;
			StateMachine.SetState( nameof( WaitState ) );
		}
	}
}
