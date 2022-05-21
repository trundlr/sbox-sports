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
		private set
		{
			if ( currentState.IsValid() )
			{
				currentState.OnExit();
			}
			currentState = value;
			if ( currentState.IsValid() )
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

	public virtual void SetState( string name )
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

	protected virtual void PreSpawnEntities( string StartState )
	{
		if ( Host.IsClient )
			return;
		var FirstPredictionState = TypeLibrary.GetAttribute<PredictStatesAttribute>( TypeLibrary.GetTypeByName( StartState ) );
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

		var predictionState = TypeLibrary.GetAttribute<PredictStatesAttribute>( entity.GetType() );
		if ( predictionState != null )
		{
			foreach ( var item in predictionState.PredictedStates )
			{
				CacheState( item );
			}
		}
	}
}
