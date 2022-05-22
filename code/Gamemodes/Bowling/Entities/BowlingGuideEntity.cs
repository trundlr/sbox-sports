namespace Sports;

// Note: Intentionally its own entity,
// We can configure the look of the particle in here
// when we have more Control Point options :)
public partial class BowlingGuideEntity : ModelEntity
{
    public Particles Particle;

    public BowlingGuideEntity()
    {
        Particle = Particles.Create( "particles/bowling/bowling_guide.vpcf", this );
    }
}