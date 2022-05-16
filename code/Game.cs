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
	public override void ClientJoined( Client cl )
	{
		base.ClientJoined( cl );

		var pawn = new BowlingPlayer();
		pawn.Respawn();

		cl.Pawn = pawn;
	}
}
