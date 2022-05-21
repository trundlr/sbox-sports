namespace Sports.StateSystem;

[Library]
[PredictStates( nameof( TurnState ) )]
public partial class WaitState : BaseState<TurnStateMachine>
{
	[Net, Predicted] public TimeSince CreationTime { get; set; }

	public override void OnEnter()
	{
		CreationTime = 0;
	}
	public override void CheckSwitchState()
	{
		if ( CreationTime > 5 )
		{
			StateMachine.TurnIndex += 1 % StateMachine.TurnOrder.Count;
			StateMachine.TurnFinished = false;
			StateMachine.SetState( nameof( TurnState ) );
		}
	}
	public override void OnTick()
	{
		if ( !Debug.Enabled )
			return;
		DebugOverlay.ScreenText( $"{(Host.IsClient ? "[CL]" : "[SV]")}WaitState: {CreationTime}", (Host.IsClient ? 0 : 1) );
	}
}

