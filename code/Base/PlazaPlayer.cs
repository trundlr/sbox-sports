using Sports.PartySystem;

namespace Sports;

public class PlazaPlayer : BasePlayer
{
	public override void Respawn()
	{
		base.Respawn();
		Camera = new ThirdPersonCamera();
	}
	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 200 ).Ignore( this ).Run();
		DebugOverlay.TraceResult( tr );
		if ( tr.Hit && tr.Entity is BasePlayer player && IsClient )
		{
			if ( Input.Pressed( InputButton.Use ) )
			{
				PartyManager.InvitePlayer( player.NetworkIdent );
			}
		}
	}
}
