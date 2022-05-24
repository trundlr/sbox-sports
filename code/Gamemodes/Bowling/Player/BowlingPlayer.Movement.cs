namespace Sports;

public partial class BowlingPlayer
{
	public BowlingMoveType CurrentMoveType = BowlingMoveType.Move;

	private void SimulateMovement( Client cl )
	{
		float inputLeft = 0;

		if ( !HasThrown )
		{
			// Switch between move types
			if ( Input.Released( InputButton.Reload ) )
				CurrentMoveType = CurrentMoveType == BowlingMoveType.Move ? BowlingMoveType.Rotate : BowlingMoveType.Move;

			if ( CurrentMoveType == BowlingMoveType.Move )
				inputLeft = Input.Left;

			if ( CurrentMoveType == BowlingMoveType.Rotate )
				Rotation *= Rotation.FromYaw( Input.Left * 2 );
		}

		if ( Debug.Enabled )
			DebugOverlay.ScreenText( $"[BOWLING MOVEMENT]\n Move type: {CurrentMoveType}" );
	}
}
