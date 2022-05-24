namespace Sports;

public interface IGamemodeEntity
{
	public abstract string GamemodeName { get; set; }
	public abstract BaseGamemode Gamemode { get; set; }
}
