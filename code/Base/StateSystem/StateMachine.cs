namespace Sports.StateSystem;

public partial class StateMachine : Entity
{
	[Net]
	public Dictionary<string, BaseState> States { get; set; }

	[Net]
	public BaseGamemode Gamemode { get; set; }

	[Net, Predicted]
	private BaseState _CurrentState { get; set; }

	public BaseState CurrentState
	{
		get
		{
			return _CurrentState;
		}
		private set
		{
			if ( _CurrentState.IsValid() )
			{
				_CurrentState.OnExit();
			}
			_CurrentState = value;
			if ( _CurrentState.IsValid() )
			{
				CurrentState.StateMachine = this;
				_CurrentState.OnEnter();
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

	protected virtual void PreSpawnEntities()
	{
		if ( Host.IsClient )
			return;

		Type genericPredictType = typeof( PredictedBaseState<> ).MakeGenericType( GetType() );

		var predictStates = TypeLibrary.GetTypes( genericPredictType );

		foreach ( var item in predictStates )
		{
			CacheState( item.Name );
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
