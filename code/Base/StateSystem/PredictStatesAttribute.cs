namespace Sports.StateSystem;

[AttributeUsage( AttributeTargets.Class )]
public class PredictStatesAttribute : Attribute
{
	public List<string> PredictedStates { get; private set; }
	public PredictStatesAttribute( params string[] states )
	{
		PredictedStates = new List<string>( states );
	}
}
