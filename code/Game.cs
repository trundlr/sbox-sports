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
	[Net] public Gamemode Gamemode { get; set; }

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Gamemode?.Simulate( cl );
	}

	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		Gamemode?.OnClientJoined( cl );
	}

	public override void ClientDisconnect( Client cl, NetworkDisconnectionReason reason )
	{
		base.ClientDisconnect( cl, reason );

		Gamemode?.OnClientDisconnected( cl, reason );
	}
}
