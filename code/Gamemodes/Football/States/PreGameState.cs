using Sports.StateSystem;

namespace Sports.Football.States;

public partial class FootballPreGameState : PredictedBaseState<FootballStateMachine>
{
	[Net, Predicted]
	public TimeSince StartTime { get; set; }

	public override void CheckSwitchState()
	{
		base.CheckSwitchState();

		if ( StartTime > 2 )
		{
			StateMachine.SetState( nameof( FootballGameState ) );
			StateMachine.Game.SpawnBall();
		}
	}

	public override void OnEnter()
	{
		base.OnEnter();
		StartTime = 0;
	}
}
