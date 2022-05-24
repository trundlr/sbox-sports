namespace Sports;

public partial class BowlingPitTrigger : BaseTrigger
{
	[Net, Predicted]
	public BaseGamemode Gamemode { get; set; }

	[Property( "gamemode_name", Title = "Gamemode Name" ), FGDType( "target_destination" )]
	public string GamemodeName { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		LinkGamemode();
	}

	private void LinkGamemode()
	{
		var gamemode = All.OfType<BaseGamemode>().First( x => x.Name == GamemodeName );
		if ( !gamemode.IsValid() )
		{
			Log.Warning( $"Entity {ClassName} failed to link with valid gamemode!" );
			return;
		}

		Gamemode = gamemode;
	}
}
