namespace Sports.Football;

[HammerEntity]
[Library( "sports_football_ball_spawner" )]
[EditorModel( "models/football/football_ball.vmdl" )]
public class BallSpawner : GamemodeEntity
{
	public SoccerBall SpawnBall()
	{
		return new SoccerBall
		{
			Transform = Transform
		};
	}
}
