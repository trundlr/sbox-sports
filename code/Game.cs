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
	[Net] public BaseGamemode Gamemode { get; set; }
	public static SportsGame Instance => Current as SportsGame;

	/// <summary>
	/// Called on a pawn's Initial respawn.
	/// </summary>
	/// <param name="pawn">The Pawn that was respawned.</param>
	public void OnPawnJoined( BasePlayer pawn )
	{
		Gamemode?.OnPawnJoined( pawn );
	}

	/// <summary>
	/// Called when a pawn respawns.
	/// </summary>
	/// <param name="pawn">The Pawn that was respawned.</param>
	public void OnPawnRespawned( BasePlayer pawn )
	{
		Gamemode?.OnPawnRespawned( pawn );
	}

	public override void MoveToSpawnpoint( Entity pawn )
	{
		if ( pawn is BasePlayer player )
			Gamemode?.MovePawnToSpawnpoint( player );
	}

	public void OnPawnDamaged( BasePlayer pawn, DamageInfo dmg )
	{
		Gamemode?.OnPawnDamaged( pawn, dmg );
	}

	public void OnPawnKilled( BasePlayer pawn )
	{
		Gamemode?.OnPawnKilled( pawn );
	}

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

	public override void BuildInput( InputBuilder input )
	{
		base.BuildInput( input );

		Gamemode?.BuildInput( input );
	}
}
