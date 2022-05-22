namespace Sports;

public partial class BaseCamera : CameraMode
{
    protected float BaseFOV { get; set; } = 70f;

    public BaseCamera()
    {
        FieldOfView = 70f;
    }
    public override void Update()
    {
    }

    public override void Build( ref CameraSetup camSetup )
    {
        BaseFOV = camSetup.FieldOfView;

        base.Build( ref camSetup );
    }
}
