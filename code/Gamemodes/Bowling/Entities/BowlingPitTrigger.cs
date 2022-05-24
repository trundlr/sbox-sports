namespace Sports;

[Solid]
[HammerEntity]
[Category( "Bowling" )]
[Title( "Pit Trigger" )]
public partial class BowlingPitTrigger : GamemodeBaseTrigger
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

		Log.Debug( "pit touch start" );

		if ( toucher is BowlingBall ball )
		{
			Log.Debug( $"ball start touch {ball}" );
		}
	}

	public override void EndTouch( Entity other )
	{
		base.EndTouch( other );

		if ( other is BowlingBall ball )
		{
			Log.Debug( $"ball end touch {ball}" );
		}
	}
}
