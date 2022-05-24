namespace Sports;

/// <summary>
/// Used to trigger generic events around the pin group.
/// Should encompass the pin group fully, with some margin, to be able to trigger events as possible spare/strikes happen.
/// </summary>
[Solid]
[HammerEntity]
[Category( "Bowling" )]
[Title( "Event Trigger" )]
[AutoApplyMaterial( "materials/bowling/tools/event_trigger.vmat" )]
public class BowlingEventTrigger : GamemodeBaseTrigger
{
	public override void Spawn()
	{
		base.Spawn();

		Transmit = TransmitType.Always;

		ActivationTags.Add( "bowling_ball" );
	}

	public override void OnTouchStart( Entity toucher )
	{
		base.OnTouchStart( toucher );

		if ( toucher is BowlingBall ball )
		{
			Log.Debug( $"event triggr ball start touch {ball}" );
		}
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is BowlingBall ball )
		{
			Log.Debug( $"event trigger ball end touch {ball}" );
		}
	}
}
