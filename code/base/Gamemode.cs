namespace Sports;

public abstract partial class BaseGamemode : Entity
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

	public virtual void OnPawnJoined( BasePlayer pawn ) { }

	public virtual void OnPawnRespawned( BasePlayer pawn ) { }

	public virtual void MovePawnToSpawnpoint( BasePlayer pawn ) { }

	public virtual void OnPawnDamaged( BasePlayer pawn, DamageInfo dmg ) { }

	public virtual void OnPawnKilled( BasePlayer pawn ) { }
}
