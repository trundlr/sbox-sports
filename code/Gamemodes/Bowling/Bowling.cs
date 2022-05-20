namespace Sports;

[Library( "sports_bowling_gamemode" )]
[EditorModel( "models/Bowling/bpin.vmdl" )]
[Display( Name = "Bowling Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class Bowling : BaseGamemode
{
	public override BasePlayer CreatePawn()
	{
		return new BowlingPlayer();
	}
}
