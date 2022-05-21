namespace Sports;

[HammerEntity]
[Title( "Gamemode Point" )]
[Category( "Gamemode Setup" )]
[Icon( "transform" )]
[EditorModel( "models/editor/camera.vmdl" )]
public partial class GamemodePointEntity : Entity
{
	[Net]
	public BaseGamemode Gamemode { get; set; }

	[Property, Title( "Gamemode ID" )]
	public string GamemodeId { get; set; }

	public override string ToString()
	{
		return $"Gamemode Point [{GamemodeId}] ({string.Join( ", ", Tags.List )})";
	}

	public override void Spawn()
	{
		base.Spawn();

		// Ideally we'd transmit this to participants of the gamemode but we don't really
		// have control over that just yet.
		Transmit = TransmitType.Always;
	}
}
