using Sports.StateSystem;

namespace Sports;

public partial class TurnStateMachine : StateMachine
{
	public List<TurnComponent> TurnOrder { get; set; } = new(); // TODO: possibly make this a list of entities instead of components, so it doesn't complain when Networked

	public TurnComponent CurrentTurn
	{
		get
		{
			if ( Host.IsClient || TurnOrder == null || TurnOrder.Count == 0 )
				return null;
			if ( TurnIndex >= TurnOrder.Count )
			{
				TurnIndex = 0;
			}
			return TurnOrder[TurnIndex];
		}
	}

	[Net] public int TurnIndex { get; set; }

	public void StartGame()
	{
		TurnOrder.Clear();
		foreach ( var item in Entity.Clients )
		{
			var TurnComponent = item.Components.GetOrCreate<TurnComponent>();
			TurnComponent.HasTurn = false;
			TurnOrder.Add( TurnComponent );
		}
		TurnIndex = 0;
		CurrentTurn.HasTurn = true; // give the first player a turn
		CurrentState = new TurnState();
	}
	public void EndGame()
	{
		CurrentState = new LobbyState();
	}
	public override void Update()
	{
		base.Update();
		if ( !Debug.Enabled )
			return;

		DebugOverlay.ScreenText( $"TurnStateMachine: {CurrentTurn?.Entity?.Name}, State: {CurrentState}", 8 );
	}
}
