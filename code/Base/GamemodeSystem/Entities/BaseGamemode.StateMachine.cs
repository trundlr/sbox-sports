using Sports.StateSystem;

namespace Sports;

public partial class BaseGamemode : Entity
{
	[Net]
	public StateMachine StateMachine { get; protected set; }

	public virtual void SetStateMachine( StateMachine stateMachine, string StartState = "" )
	{
		if ( IsClient ) // Clients shouldn't create state machines
			return;

		StateMachine = stateMachine;
		StateMachine.Gamemode = this;
		StateMachine.Parent = this;

		if ( !string.IsNullOrEmpty( StartState ) )
			StateMachine.SetState( StartState );
	}

	public virtual void SetStateMachine<T>( string StartState = "" ) where T : StateMachine, new()
	{
		if ( IsClient ) // Clients shouldn't create state machines
			return;

		SetStateMachine( new T(), StartState );
	}
}
