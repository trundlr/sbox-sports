namespace Sports;

[HammerEntity]
[Library( "sports_test_gamemode" )]
[EditorModel( "models/Bowling/bpin.vmdl" )]
[Title( "Test Gamemode" )]
[Category( "Gamemodes" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class TestGamemode : BaseGamemode
{
	public override BasePlayer CreatePawn()
	{
		return new PlazaPlayer();
	}
}
