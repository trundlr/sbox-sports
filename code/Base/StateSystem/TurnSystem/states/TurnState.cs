namespace Sports.StateSystem;

public class TurnState : BaseState<TurnStateMachine>
{
	public override void CheckSwitchState()
	{
		Log.Debug( $"PlayerTurnFinished: {StateMachine.CurrentTurn.TurnFinished}" );
		if ( StateMachine.CurrentTurn.TurnFinished )
		{
			StateMachine.CurrentTurn.TurnFinished = false;
			StateMachine.CurrentState = new WaitState();
		}
	}
	public override void OnEnter()
	{
		base.OnEnter();
		foreach ( var player in StateMachine.TurnOrder )
		{
			player.HasTurn = player == StateMachine.CurrentTurn;
		}
	}
}
