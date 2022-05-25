namespace Sports.Football;

[HammerEntity]
[Library( "sports_football_goal" )]
[Title( "Goal Trigger" ), Icon( "select_all" )]
public class Goal : GamemodeBaseTrigger
{

	public FootballGame GamemodeInstance => Gamemode as FootballGame;

	public override void OnTouchStart( Entity toucher )
	{
		if ( toucher is Ball ball )
		{
			GamemodeInstance?.OnGoal( this );
		}
	}
}
