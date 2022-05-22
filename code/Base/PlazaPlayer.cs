using Sports.UI;

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

		if ( Debug.Enabled )
			DebugOverlay.TraceResult( tr );

		if ( tr.Hit && tr.Entity is Entity entity && IsClient )
		{
			if ( Input.Pressed( InputButton.Use ) )
			{
				if ( entity is not IInteractable )
					return;
				var menu = new InteractionMenu();
				menu.SetEntity( entity );
				SportsGame.Instance.Hud.AddChild( menu );
			}
		}
	}
}
