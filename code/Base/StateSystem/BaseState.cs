namespace Sports.StateSystem;


public partial class BaseState : Entity
{
	[Net]
	public StateMachine StateMachine { get; set; }

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	public virtual void CheckSwitchState() { }

	public virtual void OnEnter() { }

	public virtual void OnExit() { }

	public virtual void OnTick() { }

}
