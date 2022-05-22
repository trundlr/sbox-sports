namespace Sports;

public enum BowlingCameraState
{
    Default,
    Focus,
    Throwing,
    Thrown
}

public partial class BowlingPlayerCamera : BaseCamera
{
    protected virtual float MouseWheelScale => 5f;
    protected virtual MinMax<int> BackwardsBounds => new( 0, 30 );

    protected BowlingPlayer Player => Local.Pawn as BowlingPlayer;
    protected Entity BowlingBallEntity => Player.Ball.BowlingBall.IsValid() ? Player.Ball.BowlingBall : Player.Ball;

    protected int BackwardsOffset { get; set; } = 0;
    protected Vector3 TargetPosition { get; set; } = Vector3.Zero;
    protected float PositionLerpSpeed { get; set; } = 15f;

    protected BowlingCameraState CameraState { get; set; } = BowlingCameraState.Default;

    protected void UpdateCameraDefault( BowlingPlayer player )
    {
        var center = player.Position + Vector3.Up * 58 + Rotation.Backward * 60f + Rotation.Right * 25f;

        TargetPosition = center;
        PositionLerpSpeed = 15f;
    }

    protected void UpdateCameraFocus( BowlingPlayer player )
    {
        var center = player.Position + Vector3.Up * 16 + Rotation.Backward * 60f + Rotation.Right * 25f;

        TargetPosition = center;
        PositionLerpSpeed = 12f;
    }

    protected void UpdateCameraFollowBall( BowlingPlayer player )
    {
        var center = BowlingBallEntity.Position + Vector3.Up * 16 + Rotation.Backward * 60f;

        TargetPosition = center;
        PositionLerpSpeed = 12f;
    }

    protected void UpdateCameraThrowing( BowlingPlayer player )
    {
        var center = player.Position + Vector3.Up * 16 + Rotation.Backward * 60f + Rotation.Right * 25f;

        TargetPosition = center.WithZ( BowlingBallEntity.Position.z );
        PositionLerpSpeed = 5f;
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
            case BowlingCameraState.Throwing:
                UpdateCameraThrowing( player );
                break;
            case BowlingCameraState.Thrown:
                UpdateCameraFollowBall( player );
                break;
            default:
                break;
        }

        Position = Position.LerpTo( TargetPosition, Time.Delta * PositionLerpSpeed );
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
        if ( !player.IsValid() )
            return;

        // @TODO: better way to dictate camera state. probably match a game state instead
        if ( player.HasThrown && player.Ball.BowlingBall.IsValid() )
            CameraState = BowlingCameraState.Thrown;
        else if ( player.HasThrown )
            CameraState = BowlingCameraState.Throwing;
        else if ( input.Down( InputButton.SecondaryAttack ) )
            CameraState = BowlingCameraState.Focus;
        else
            CameraState = BowlingCameraState.Default;

        base.BuildInput( input );
    }
}
