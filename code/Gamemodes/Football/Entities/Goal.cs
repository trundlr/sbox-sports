namespace Sports.Football;

[HammerEntity]
[Library( "sports_football_goal" )]
[Title( "Goal Trigger" ), Icon( "select_all" )]
public class Goal : GamemodeBaseTrigger
{

	public FootballGame GamemodeInstance => Gamemode as FootballGame;

	public override void OnTouchStart( Entity toucher )
	{
		if ( toucher is Ball )
		{
			GamemodeInstance?.OnGoal();
		}
		else if ( toucher is FootballPlayer player )
		{
			Log.Info( "Player touched goal" );
			player.Respawn(); // make players respawn if they fall into the goal
		}
	}
}
