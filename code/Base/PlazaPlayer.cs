namespace Sports;

public class PlazaPlayer : BasePlayer
{
	public override void Respawn()
	{
		base.Respawn();
		Camera = new ThirdPersonCamera();
	}
}
