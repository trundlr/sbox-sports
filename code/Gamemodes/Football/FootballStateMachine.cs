using Sports.StateSystem;

namespace Sports.Football;

public partial class FootballStateMachine : StateMachine
{

	public FootballGame Game { get => Gamemode as FootballGame; set => Gamemode = value; }

	[Net]
	public bool GameActive { get; set; } = false;

	[Net]
	public bool Goal { get; set; } = false;

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		if ( !Debug.Enabled )
			return;

		DebugOverlay.ScreenText( $"GameActive: {GameActive}, State: {CurrentState}", 1 );
	}
}
