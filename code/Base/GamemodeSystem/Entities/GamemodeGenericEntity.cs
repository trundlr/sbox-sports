namespace Sports;

/// <summary>
/// Utility entity for generic gamemode purposes.
/// </summary>
[HammerEntity]
[Category( "Gamemode Meta" )]
[Title( "Generic Gamemode Entity" )]
[EditorModel( "models/editor/info_target.vmdl" )]
public class GamemodeGenericEntity : GamemodeEntity
{
	public override void Spawn()
	{
		base.Spawn();

		// Ideally we'd transmit this to participants of the gamemode but we don't really
		// have control over that just yet.
		Transmit = TransmitType.Always;
	}

	public override string ToString()
	{
		return $"{ClassName}:[{Gamemode.ClassName}] ({string.Join( ", ", Tags.List )})";
	}
}
