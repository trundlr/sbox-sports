namespace Sports;

/// <summary>
/// Represents the group of pins in a normal 10-pin bowling game.
/// </summary>
[HammerEntity]
[Title( "Pin Group" )]
[Category( "Bowling" )]
[EditorModel( "models/bowling/pin_group.vmdl" )]
public partial class BowlingPinGroup : GamemodeModelEntity
{
	[Net]
	public IList<BowlingPin> Pins { get; set; }

	public override void Spawn()
	{
		base.Spawn();

		// set model for the sake of attachments
		SetModel( "models/bowling/pin_group.vmdl" );
		EnableDrawing = false;

		CreatePins();
	}

	public void ClearPins()
	{
		// remove all currently referenced pins
		foreach ( var pin in Pins )
		{
			if ( !pin.IsValid() || !pin.IsAuthority )
				continue;

			pin?.Delete();
		}

		Pins.Clear();
	}

	public void CreatePins()
	{
		for ( int i = 0; i < Model?.AttachmentCount; i++ )
		{
			var attachement = GetAttachment( $"pin{i}", true ).Value;

			var pin = new BowlingPin();
			pin.Transform = attachement;
			pin.InitialPosition = attachement.Position;
			pin.Index = i;
			pin.PinGroup = this;

			// slight bit of rotation variation for detail ;)
			pin.Rotation = pin.Rotation.RotateAroundAxis( pin.Rotation.Up, Rand.Float( -1, 1 ) * 15 );

			Pins.Add( pin );
		}
	}

	public void ResetPins()
	{
		ClearPins();
		CreatePins();
	}

	[Event.Tick.Server]
	public void ServerTick()
	{
		if ( !Debug.Enabled ) return;

		for ( int i = 0; i < Model?.AttachmentCount; i++ )
		{
			var name = Model.GetAttachmentName( i );
			var attachement = GetAttachment( name, true ).Value;

			DebugOverlay.Text( name, attachement.Position );
			DebugOverlay.Sphere( attachement.Position, 1, Color.White );
		}
	}
}
