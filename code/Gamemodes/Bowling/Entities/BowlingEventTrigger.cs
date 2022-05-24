namespace Sports;

[Solid]
[HammerEntity]
[Category( "Bowling" )]
[Title( "Event Trigger" )]
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
