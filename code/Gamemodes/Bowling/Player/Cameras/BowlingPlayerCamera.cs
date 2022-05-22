namespace Sports;

public enum BowlingCameraState
{
    Default,
    Focus,
    Thrown
}

public partial class BowlingPlayerCamera : BaseCamera
{
    protected virtual float MouseWheelScale => 5f;

    protected int BackwardsOffset { get; set; } = 0;

    protected MinMax<int> BackwardsBounds = new( 0, 30 );

    public Vector3 TargetPosition { get; set; } = Vector3.Zero;

    public BowlingCameraState CameraState { get; set; } = BowlingCameraState.Default;

    public BowlingPlayer Player => Local.Pawn as BowlingPlayer;

    protected void UpdateCameraDefault( BowlingPlayer player )
    {
        var center = player.Position + Vector3.Up * 58 + Rotation.Backward * 60f + Rotation.Right * 25f;

        TargetPosition = center;
    }

    protected void UpdateCameraFocus( BowlingPlayer player )
    {
        var center = player.Position + Vector3.Up * 16 + Rotation.Backward * 60f + Rotation.Right * 25f;

        TargetPosition = center;
    }

    protected void UpdateCameraFollowBall( BowlingPlayer player )
    {
        Entity targetEntity = player.Ball.BowlingBall.IsValid() ? player.Ball.BowlingBall : player.Ball;
        var center = targetEntity.Position + Vector3.Up * 16 + Rotation.Backward * 60f;

        TargetPosition = center;
    }

    public override void Update()
    {
        Viewer = null;

        var player = Player;
        if ( !player.IsValid() )
            return;

        if ( Position.IsNearlyZero() )
            Position = player.Position;

        Rotation = player.Rotation;

        switch ( CameraState )
        {
            case BowlingCameraState.Default:
                UpdateCameraDefault( player );
                break;
            case BowlingCameraState.Focus:
                UpdateCameraFocus( player );
                break;
            case BowlingCameraState.Thrown:
                UpdateCameraFollowBall( player );
                break;
            default:
                break;
        }

        Position = Position.LerpTo( TargetPosition, Time.Delta * 15f );
    }

    public override void Build( ref CameraSetup camSetup )
    {
        BaseFOV = camSetup.FieldOfView;

        base.Build( ref camSetup );
    }

    public override void BuildInput( InputBuilder input )
    {
        input.AnalogLook = Angles.Zero;

        BackwardsOffset -= (int)(input.MouseWheel * MouseWheelScale);
        BackwardsOffset = Math.Clamp( BackwardsOffset, BackwardsBounds.Min, BackwardsBounds.Max );

        var player = Player;

        if ( player.IsValid() && player.HasThrown )
            CameraState = BowlingCameraState.Thrown;
        else if ( input.Down( InputButton.SecondaryAttack ) )
            CameraState = BowlingCameraState.Focus;
        else
            CameraState = BowlingCameraState.Default;

        base.BuildInput( input );
    }
}
