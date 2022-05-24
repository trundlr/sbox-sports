using Sports.StateSystem;

namespace Sports.Football.States;

public partial class FBGameState : PredictedBaseState<FBStateMachine>
{
	[Net, Predicted]
	public float RoundTime { get; set; } = 120;
	public override void CheckSwitchState()
	{
		base.CheckSwitchState();
		if ( StateMachine.Goal )
		{
			StateMachine.SetState( nameof( FBPreGameState ) );
			StateMachine.Goal = false;
			Log.Debug( "Goal" );
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


		DebugOverlay.ScreenText( $"Time: {RoundTime}", 2 );

		if ( RoundTime <= 0 )
		{
			StateMachine.SetState( nameof( FBPostGameState ) );
		}
	}
}
