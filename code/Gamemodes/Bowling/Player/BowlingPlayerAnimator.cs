namespace Sports;

public class BowlingPlayerAnimator : PawnAnimator
{
	private BowlingPlayer BowlingPlayer => AnimPawn as BowlingPlayer;

	public override void Simulate()
	{
		var input = BowlingPlayer.CurrentMoveType == BowlingMoveType.Move ? Input.Left : 0;
		SetAnimParameter( "move_x", MathX.LerpTo( AnimPawn.GetAnimParameterFloat( "move_x" ), input * -50f, Time.Delta * 10f ) );
		Position += AnimPawn.RootMotion * Rotation * Time.Delta * 1.5f;
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
