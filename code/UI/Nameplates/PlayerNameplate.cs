namespace Sports.UI;

[UseTemplate]
public class PlayerNameplate : WorldPanel
{
	public Label NameLabel { get; set; }
	public Image AvatarImage { get; set; }

	public PlayerNameplate( Entity entity )
	{
		PanelBounds = new Rect( -250, -200, 800, 200 );
		StyleSheet.Load( "UI/Nameplates/PlayerNameplate.scss" );

		NameLabel.Text = entity.Client.Name;
		AvatarImage.SetTexture( $"avatarbig:{entity.Client.PlayerId}" );
	}

	public virtual void UpdateFromPlayer( BasePlayer player )
	{
		var localPlayer = Local.Pawn as BasePlayer;

		var angles = CurrentView.Rotation.Angles();
		angles.yaw += 180;
		angles.pitch = 0;

		var rot = angles.ToRotation();

		//
		// Where we putting the label, in world coords
		//
		var head = player.GetAttachment( "hat" ) ?? new Transform( CurrentView.Position );
		var labelPos = head.Position;

		var dist = 25f;
		var startPosRight = rot.Left * dist;

		var tr = Trace.Ray( labelPos, labelPos + (rot.Left * dist * 3) )
			.Radius( 8 )
			.Ignore( player )
			.Run();

		if ( tr.Hit )
			startPosRight = rot.Right * 40f;

		SetClass( "flipped", tr.Hit );
		SetClass( "party", Local.Client.IsSameParty( player.Client ) );

		Position = labelPos + rot.Up * -12.5f + startPosRight;
		Rotation = rot;
	}
}
