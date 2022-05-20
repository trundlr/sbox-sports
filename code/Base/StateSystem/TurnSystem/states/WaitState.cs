namespace Sports.StateSystem;

public class WaitState : BaseState<TurnStateMachine>
{
	public TimeSince CreationTime = 0;
	public override void CheckSwitchState()
	{
		if ( CreationTime > 5 )
		{
			StateMachine.TurnIndex += 1 % StateMachine.TurnOrder.Count;
			StateMachine.CurrentState = new TurnState();
		}
	}
	public override void OnTick()
	{
		if ( !Debug.Enabled )
			return;
		DebugOverlay.ScreenText( $"WaitState: {CreationTime}" );
	}
}

