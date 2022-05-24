using Sports.StateSystem;

namespace Sports;

public partial class TurnStateMachine : StateMachine
{

	[Net]
	public List<Client> TurnOrder { get; set; } = new();

	[Net, Predicted]
	public bool TurnFinished { get; set; } = false;
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
		PreSpawnEntities();
	}

	public override void OnGamemodeStart()
	{
		base.OnGamemodeStart();

		TurnOrder.Clear();
		foreach ( var item in Gamemode.Clients )
		{
			TurnOrder.Add( item );
		}
		TurnIndex = 0;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		if ( !Debug.Enabled )
			return;
		DebugOverlay.ScreenText( $"TurnStateMachine: {CurrentTurn?.Name}, State: {CurrentState}", 15 );
	}

	public virtual void EndTurn()
	{
		TurnFinished = true;
	}

}
