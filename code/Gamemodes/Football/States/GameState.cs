using Sports.StateSystem;

namespace Sports.Football.States;

public partial class FootballGameState : PredictedBaseState<FootballStateMachine>
{

	[Net, Predicted]
	public float RoundTime { get; set; }

	public override void CheckSwitchState()
	{
		base.CheckSwitchState();
		if ( StateMachine.Goal )
		{
			StateMachine.SetState( nameof( FootballPreGameState ) );
			StateMachine.Goal = false;
		}
	}

	public override void OnEnter()
	{
		base.OnEnter();
		if ( !StateMachine.GameActive )
		{
			RoundTime = 120;
			StateMachine.GameActive = true;
		}
	}

	public override void OnTick()
	{
		RoundTime -= Time.Delta;

		if ( Debug.Enabled )
			DebugOverlay.ScreenText( $"Time: {RoundTime}", 2 );

		if ( RoundTime <= 0 )
		{
			StateMachine.SetState( nameof( FootballPostGameState ) );
		}
	}
}
