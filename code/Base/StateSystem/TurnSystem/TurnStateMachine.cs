using Sports.StateSystem;

namespace Sports;

public partial class TurnStateMachine : StateMachine
{
	[Net] public List<Client> TurnOrder { get; set; } = new(); // TODO: possibly make this a list of entities instead of components, so it doesn't complain when Networked

	[Net, Predicted] public bool TurnFinished { get; set; } = false;
	public Client CurrentTurn
	{
		get
		{
			if ( TurnOrder == null || TurnOrder.Count == 0 )
				return null;
			if ( TurnIndex >= TurnOrder.Count )
			{
				TurnIndex = 0;
			}
			return TurnOrder[TurnIndex];
		}
	}

	[Net, Predicted] public int TurnIndex { get; set; }
	public override void Spawn()
	{
		base.Spawn();
		PreSpawnEntities( nameof( LobbyState ) );
	}
	public void StartGame()
	{
		TurnOrder.Clear();
		foreach ( var item in Gamemode.Clients )
		{
			TurnOrder.Add( item );
		}
		TurnIndex = 0;
		SetState( nameof( TurnState ) );
	}
	public void EndGame()
	{
		SetState( nameof( LobbyState ) );
	}
	public override void Simulate( Client cl )
	{
		if ( Input.Pressed( InputButton.Duck ) )
		{
			TurnFinished = true;
		}
		base.Simulate( cl );
		if ( !Debug.Enabled )
			return;


		DebugOverlay.ScreenText( $"TurnStateMachine: {CurrentTurn?.Name}, State: {CurrentState}", 8 );
	}



}
