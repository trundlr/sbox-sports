namespace Sports.StateSystem;

public partial class StateMachine : EntityComponent<BaseGamemode>
{
	[Net] private BaseState currentState { get; set; }
	public BaseState CurrentState
	{
		get
		{
			return currentState;
		}
		set
		{
			if ( currentState != null )
				currentState.OnExit();
			currentState = value;
			if ( currentState != null )
			{
				CurrentState.StateMachine = this;
				currentState.OnEnter();
			}
		}
	}

	public override bool CanAddToEntity( Entity entity ) // on hotload it errors for somereason.
	{
		return true;
	}

	[Event.Tick.Server]
	public virtual void Update()
	{
		CurrentState?.CheckSwitchState();
		CurrentState?.OnTick();
	}

}
