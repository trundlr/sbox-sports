namespace Sports;

public abstract partial class Gamemode : Entity
{
	public override void Spawn()
	{
		// for the inspector
		Name = ClassInfo.Name;

		Transmit = TransmitType.Always;
	}

	public void Start()
	{
		OnStart();
	}

	public virtual void OnStart() { }

	public void Finish()
	{
		OnFinish();
	}

	public virtual void OnFinish() { }

	public virtual void OnClientJoined( Client cl ) { }

	public virtual void OnClientDisconnected( Client cl, NetworkDisconnectionReason reason ) { }
}
