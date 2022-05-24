namespace Sports;

[HammerEntity]
[Title( "Generic Gamemode AnimatedEntity" )]
[Category( "Gamemode Setup" )]
[Icon( "transform" )]
[EditorModel( "models/editor/info_target.vmdl" )]
public partial class GamemodeAnimatedEntity : AnimatedEntity, IGamemodeEntity
{
	[Property( Title = "Gamemode Name" ), Net]
	public string GamemodeName { get; set; }
	[Net]
	public BaseGamemode Gamemode { get; set; }

	public override string ToString()
	{
		return $"{ClassName}:[{Gamemode.ClassName}] ({string.Join( ", ", Tags.List )})";
	}

	public override void Spawn()
	{
		base.Spawn();

		// Ideally we'd transmit this to participants of the gamemode but we don't really
		// have control over that just yet.
		Transmit = TransmitType.Always;
	}
}
