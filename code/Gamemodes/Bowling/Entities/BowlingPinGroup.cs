namespace Sports;

[HammerEntity]
[EditorModel( "models/bowling/pin_group.vmdl" )]
public class BowlingPinGroup : Entity
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;
	}

	[Event.Tick.Server]
	public void ServerTick()
	{
		if ( !Debug.Enabled ) return;

		DebugOverlay.Text( "pingroup", Position );
		DebugOverlay.Sphere( Position, 5, Color.White );
	}
}
