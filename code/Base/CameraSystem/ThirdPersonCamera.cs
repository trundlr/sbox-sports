namespace Sports;

public partial class ThirdPersonCamera : BaseCamera
{
    [ConVar.Replicated( "sports_thirdperson_orbit_enabled" )]
    public static bool ShouldOrbit { get; set; } = false;

    [ConVar.Replicated( "sports_thirdperson_collision_enabled" )]
    public static bool ShouldCollide { get; set; } = true;

    public float BaseFOVOffset { get; set; } = -10f;
    public float FocusFOVOffset { get; set; } = -20f;

    protected float FOVAdjustSpeed { get; set; } = 10f;
    protected float OrbitDistance { get; set; } = 150f;
    protected float InternalFOVOffset { get; set; } = 0f;

    protected Angles OrbitAngles;

    public override void Update()
    {
        var pawn = Local.Pawn as AnimatedEntity;
        if ( !pawn.IsValid() )
            return;

        Position = pawn.Position;
        Vector3 targetPos;

        var center = pawn.Position + Vector3.Up * 58;

        if ( ShouldOrbit )
        {
            Position += Vector3.Up * (pawn.CollisionBounds.Center.z * pawn.Scale);
            Rotation = Rotation.From( OrbitAngles );

            targetPos = Position + Rotation.Backward * OrbitDistance;
        }
        else
        {
            Position = center;
            Rotation = Rotation.FromAxis( Vector3.Up, 4 ) * Input.Rotation;

            float distance = 100.0f * pawn.Scale;
            targetPos = Position + Input.Rotation.Right * ((pawn.CollisionBounds.Maxs.x + 20) * pawn.Scale);
            targetPos += Input.Rotation.Forward * -distance;
        }

        if ( ShouldCollide )
        {
            var tr = Trace.Ray( Position, targetPos )
                .Ignore( pawn )
                .Radius( 8 )
                .Run();

            Position = tr.EndPosition;
        }
        else
        {
            Position = targetPos;
        }

        Viewer = null;

        FieldOfView = FieldOfView.LerpTo( BaseFOV + InternalFOVOffset, Time.Delta * FOVAdjustSpeed );
    }

    public override void BuildInput( InputBuilder input )
    {
        if ( ShouldOrbit && input.Down( InputButton.Walk ) )
        {
            if ( input.Down( InputButton.PrimaryAttack ) )
            {
                OrbitDistance += input.AnalogLook.pitch;
                OrbitDistance = OrbitDistance.Clamp( 0, 1000 );
            }
            else
            {
                OrbitAngles.yaw += input.AnalogLook.yaw;
                OrbitAngles.pitch += input.AnalogLook.pitch;
                OrbitAngles = OrbitAngles.Normal;
                OrbitAngles.pitch = OrbitAngles.pitch.Clamp( -89, 89 );
            }

            input.AnalogLook = Angles.Zero;

            input.Clear();
            input.StopProcessing = true;
        }

        InternalFOVOffset = input.Down( InputButton.SecondaryAttack ) ? FocusFOVOffset : BaseFOVOffset;

        base.BuildInput( input );
    }
}
