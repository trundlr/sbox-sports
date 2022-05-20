namespace Sports.UI;

[UseTemplate]
public partial class SportsChatEntry : Panel
{
	public Label ChatType { get; internal set; }
	public Label NameLabel { get; internal set; }
	public Label Message { get; internal set; }
	public Image Avatar { get; internal set; }

	public RealTimeSince TimeSinceCreated = 0;

	public SportsChatEntry()
	{
	}

	public override void Tick()
	{
		base.Tick();

		if ( TimeSinceCreated > 30 )
		{
			Delete();
		}
	}
}
