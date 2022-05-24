namespace Sports.Football;

[HammerEntity]
[Library( "sports_football_ball_spawner" )]
[EditorModel( "models/football/football_ball.vmdl" )]
public class BallSpawner : Entity
{
	public Ball SpawnBall()
	{
		return new Ball
		{
			Transform = Transform
		};
	}
}
