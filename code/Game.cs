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

namespace Sports;

public partial class SportsGame : Game
{
	[Net] public IList<BaseGamemode> Gamemodes { get; set; }
	public static SportsGame Instance => Current as SportsGame;

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
