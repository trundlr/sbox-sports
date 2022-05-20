global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using SandboxEditor;
global using System;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Linq;

using Sports.UI;

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

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		foreach ( var gamemode in Gamemodes )
		{
			gamemode.FrameSimulate( cl );
		}
	}

	public override void ClientJoined( Client cl )
	{
		Log.Info( $"{cl.Name} has joined the session" );
		SportsChatBox.AddInformation( To.Everyone, $"{cl.Name} joined the session", $"avatar:{cl.PlayerId}" );

		// Give the client the ability to be referenced to a specific gamemode
		cl.GetGamemodeComponent();

		// Give the client the PartyComponent
		cl.GetPartyComponent();

		// Set up the default pawn
		SetupDefaultPawn( cl );
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		Log.Info( $"{cl.Name} has left the session ({reason})" );
		SportsChatBox.AddInformation( To.Everyone, $"{cl.Name} left the session", $"avatar:{cl.PlayerId}" );

		if ( cl.Pawn.IsValid() )
		{
			cl.Pawn.Delete();
			cl.Pawn = null;
		}

		var gamemode = cl.GetGamemode();
		if ( gamemode.IsValid() )
		{
			gamemode.RemoveClient( cl, reason.ToLeaveReason() );
		}

		cl.GetPartyComponent()?.Leave();
	}
}
