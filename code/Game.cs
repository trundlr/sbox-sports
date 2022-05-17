global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using Sandbox.Component;
global using Hammer;

global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.ComponentModel;
global using System.Threading.Tasks;
global using System.ComponentModel.DataAnnotations;

namespace Sports;

public partial class SportsGame : Game
{
	[Net] public IList<BaseGamemode> Gamemodes { get; set; }
	public static SportsGame Instance => Current as SportsGame;

	// Clientside HUD
	public SportsHud Hud { get; set; }

	public BasePlayer CreatePawn() => new PlazaPlayer();

	/// <summary>
	/// Set up the default pawn for when the player is not in a gamemode
	/// </summary>
	/// <param name="cl"></param>
	public void SetupDefaultPawn( Client cl )
	{
		cl.Pawn?.Delete();
		var PlayerPawn = CreatePawn();
		cl.Pawn = PlayerPawn;
		PlayerPawn.InitialSpawn();
	}

	public SportsGame()
	{
		// Create HUD clientside
		if ( Host.IsClient )
		{
			Hud = new();
		}
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		foreach ( var gamemode in Gamemodes )
		{
			gamemode.Simulate( cl );
		}
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		// Give the client the ability to be referenced to a specific gamemode
		GamemodeEntityComponent.GetOrCreate( cl );

		// Set up the default pawn
		SetupDefaultPawn( cl );
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		var component = GamemodeEntityComponent.GetOrCreate( cl );
		var gamemode = component.Gamemode;

		if ( gamemode.IsValid() )
		{
			gamemode.RemoveClient( cl, reason.ToLeaveReason() );
		}
	}
}
