using Sandbox;

namespace Sports;

public class BowlingPlayerAnimator : PawnAnimator
{
	private BowlingMoveType CurrentMoveType = BowlingMoveType.Move;

	public override void Simulate()
	{
		float inputLeft = 0;

		// Switch between move types
		if ( Input.Released( InputButton.Reload ) )
			CurrentMoveType = CurrentMoveType == BowlingMoveType.Move ? BowlingMoveType.Rotate : BowlingMoveType.Move;

		if ( CurrentMoveType == BowlingMoveType.Move )
			inputLeft = Input.Left;

		if ( CurrentMoveType == BowlingMoveType.Rotate )
			Rotation *= Rotation.FromYaw( Input.Left * 2 );

		SetAnimParameter( "move_x", MathX.LerpTo( AnimPawn.GetAnimParameterFloat( "move_x" ), inputLeft * -50f, Time.Delta * 10f ) );
		Position += AnimPawn.RootMotion * Rotation * Time.Delta * 1.5f;

		if ( Debug.Enabled )
			DebugOverlay.ScreenText( "[BOWLING MOVEMENT]\n" +
				$"Move type: {CurrentMoveType}"
				);
	}

	public void DoResultAnimation( bool wasGoodBowl )
	{
		if ( wasGoodBowl )
		{
			SetAnimParameter( "b_bowling_positive_hit", true );
			return;
		}

		SetAnimParameter( "b_bowling_negative_hit", true );
	}

	public void DoThrow()
	{
		SetAnimParameter( "b_bowling_throw", true );
	}
}
