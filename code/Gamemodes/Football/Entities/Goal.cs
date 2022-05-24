namespace Sports.Football;

[HammerEntity]
[Library( "sports_football_goal" )]
[Title( "Goal Trigger" ), Icon( "select_all" )]
public class Goal : BaseTrigger
{
	[Property,FGDType("target_destination")]
	public string Gamemode { get; set; }

	public FootballGame GamemodeInstance { get; set; }
	[Event.Entity.PostSpawn]
	public void OnSpawn()
	{
		if ( GamemodeInstance == null )
		{
			GamemodeInstance = Entity.FindByName( Gamemode ) as FootballGame;
		}
	}

	public override void OnTouchStart( Entity toucher )
	{
		if ( toucher is Ball ball )
		{
			GamemodeInstance?.OnGoal( this );
		}
	}
}
