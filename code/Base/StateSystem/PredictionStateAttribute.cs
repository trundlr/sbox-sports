namespace Sports.StateSystem;

[AttributeUsage( AttributeTargets.Class )]
public class PredictionStateAttribute : Attribute
{
	public List<string> PredictedStates { get; private set; }
	public PredictionStateAttribute( params string[] states )
	{
		PredictedStates = new List<string>( states );
	}
}
