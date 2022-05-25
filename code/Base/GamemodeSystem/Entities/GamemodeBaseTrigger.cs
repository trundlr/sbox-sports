namespace Sports;

/// <summary>
/// A generic gamemode BaseTrigger. To be inherited from for entities that have to be directly linked to a gamemode instance.
/// </summary>
public abstract partial class GamemodeBaseTrigger : BaseTrigger, IGamemodeEntity
{
	/// <summary>
	/// The targetname of the linked gamemode entity.
	/// </summary>
	[Property( Title = "Gamemode Name" ), Net, FGDType( "target_destination" )]
	public string GamemodeName { get; set; }
	[Net]
	public BaseGamemode Gamemode { get; set; }
}
