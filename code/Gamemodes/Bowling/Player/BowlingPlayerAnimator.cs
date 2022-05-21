using Sandbox;

namespace Sports;

public class BowlingPlayerAnimator : PawnAnimator
{
	public override void Simulate()
	{
		SetAnimParameter( "move_x", MathX.LerpTo( AnimPawn.GetAnimParameterFloat( "move_x" ), Input.Left * -50f, Time.Delta * 10f ) );

		Position += Rotation * AnimPawn.RootMotion * Time.Delta * 1.5f;
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
