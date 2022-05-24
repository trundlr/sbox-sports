namespace Sports;

public partial class GamemodeEntity : Entity, IGamemodeEntity
{
	/// <summary>
	/// The targetname of the linked gamemode entity.
	/// </summary>
	[Property( Title = "Gamemode Name" ), Net, FGDType( "target_destination" )]
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
