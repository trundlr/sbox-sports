namespace Sports.StateSystem;

public partial class StateMachine : Entity
{

	[Net] public Dictionary<string, BaseState> States { get; set; }
	[Net] public BaseGamemode Gamemode { get; set; }
	[Net, Predicted] private BaseState currentState { get; set; }
	public BaseState CurrentState
	{
		get
		{
			return currentState;
		}
		set
		{
			if ( currentState != null )
			{
				currentState.OnExit();
			}
			currentState = value;
			if ( currentState != null )
			{
				CurrentState.StateMachine = this;
				currentState.OnEnter();
			}
		}
	}
	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		CurrentState?.OnTick();
		CurrentState?.CheckSwitchState();
	}
	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );
	}

	public void SetState( string name )
	{
		if ( States.ContainsKey( name ) )
		{
			CurrentState = States[name];
		}
		else if ( Host.IsServer )
		{
			CurrentState = TypeLibrary.Create<BaseState>( name );
			CurrentState.Parent = this;
			States.Add( name, CurrentState );
		}
	}

	protected void PreSpawnEntities( string StartState )
	{
		if ( Host.IsClient )
			return;
		var FirstPredictionState = TypeLibrary.GetAttribute<PredictionStateAttribute>( TypeLibrary.GetTypeByName( StartState ) );
		if ( !States.ContainsKey( StartState ) )
			CacheState( StartState );
		foreach ( var item in FirstPredictionState.PredictedStates )
		{
			CacheState( item );
		}
	}

	private void CacheState( string name )
	{
		if ( States.ContainsKey( name ) )
			return;
		var entity = TypeLibrary.Create<BaseState>( name );
		entity.Parent = this;
		entity.StateMachine = this;
		States.Add( name, entity );
	}
}
