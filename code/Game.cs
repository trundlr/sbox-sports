global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Linq;

namespace Sports;

public partial class SportsGame : Game
{
	[Net] public IList<BaseGamemode> Gamemodes { get; set; }

	public BaseGamemode GetGamemodeFromId( string name )
	{
		return Gamemodes.FirstOrDefault( x => x.Name.ToLower() == name.ToLower() );
	}

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
		var pawn = CreatePawn();
		cl.Pawn = pawn;
		pawn.InitialSpawn();
	}

	public SportsGame()
	{
		// Create HUD clientside
		if ( Host.IsClient )
		{
			Hud = new();
		}
		if ( Host.IsServer )
		{
			var partyManager = new PartySystem.PartyManager();
			partyManager.Transmit = TransmitType.Always;
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
		cl.GetGamemodeComponent();

		// Give the client the PartyComponent
		cl.GetPartyComponent();

		// Set up the default pawn
		SetupDefaultPawn( cl );
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		var gamemode = cl.GetGamemode();
		if ( gamemode.IsValid() )
		{
			gamemode.RemoveClient( cl, reason.ToLeaveReason() );
		}

		cl.GetPartyComponent()?.Leave();
	}
}
