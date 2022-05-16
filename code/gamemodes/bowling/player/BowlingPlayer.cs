namespace Sports;

public class BowlingPlayer : Player
{
	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		CameraMode = new FirstPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		base.Respawn();
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Entity.All.Where( x => x.Owner == this ).OfType<BowlingBall>().ToList().ForEach( x => x.Simulate() );

		if ( !IsServer )
			return;

		if ( Input.Pressed( InputButton.Attack1 ) )
		{
			var bowlingBall = new BowlingBall();
			var tr = Trace.Ray( EyePosition, EyePosition + EyeRotation.Forward * 64 ).Ignore( this ).Run();

			bowlingBall.Position = tr.EndPosition + tr.Normal * 32;
			bowlingBall.Velocity = EyeRotation.Forward * 512;
			bowlingBall.Owner = this;
		}
	}
}
