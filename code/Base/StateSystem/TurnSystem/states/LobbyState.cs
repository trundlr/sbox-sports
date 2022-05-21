namespace Sports.StateSystem;

[Library]
[PredictStates( nameof( TurnState ) )]
public partial class LobbyState : BaseState<TurnStateMachine>
{
}
