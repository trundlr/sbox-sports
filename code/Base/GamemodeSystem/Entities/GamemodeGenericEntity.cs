namespace Sports;

/// <summary>
/// Utility entity for generic gamemode purposes.
/// This entity will be gathered automatically by its linked gamemode and can be used via its tags and generic information for meta purposes.
/// </summary>
[HammerEntity]
[Icon( "transform" )]
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
