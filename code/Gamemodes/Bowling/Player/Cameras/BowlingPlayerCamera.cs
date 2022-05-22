namespace Sports;

public partial class BowlingPlayerCamera : BaseCamera
{
    protected virtual float MouseWheelScale => 5f;

    public int BackwardsOffset { get; set; } = 0;

    public MinMax<int> Clamp = new( 0, 30 );

    public override void Update()
    {
        var pawn = Local.Pawn as AnimatedEntity;
        if ( !pawn.IsValid() )
            return;

        Rotation = pawn.Rotation;

        var center = pawn.Position + Vector3.Up * 58 + Rotation.Backward * 60f + Rotation.Right * 25f;

        Position = center + pawn.Rotation.Backward * BackwardsOffset;

        Viewer = null;
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
        BackwardsOffset = Math.Clamp( BackwardsOffset, Clamp.Min, Clamp.Max );

        base.BuildInput( input );
    }
}
