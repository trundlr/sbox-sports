using Sports.StateSystem;

namespace Sports.Football.States;

public partial class FBPreGameState : PredictedBaseState<FBStateMachine>
{
	[Net, Predicted]
	public TimeSince StartTime { get; set; }
	public override void CheckSwitchState()
	{
		base.CheckSwitchState();

		if ( StartTime > 2 )
		{
			StateMachine.SetState( nameof( FBGameState ) );
			StateMachine.Game.SpawnBall();
		}
	}

	public override void OnEnter()
	{
		base.OnEnter();
		StartTime = 0;
	}
}
