namespace Sports.StateSystem;

public partial class TurnComponent : EntityComponent
{
	[Net] public bool HasTurn { get; set; }
	[Net] public bool TurnFinished { get; set; }

	[ConCmd.Server]
	public static void EndTurn()
	{
		if ( ConsoleSystem.Caller is Client cl )
		{
			cl.GetGamemode().Clients.First( e => e.Components.Get<TurnComponent>().HasTurn ).Components.Get<TurnComponent>().TurnFinished = true;
		}
	}
}
