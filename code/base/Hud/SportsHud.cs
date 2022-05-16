namespace Sports;

public class SportsHud : RootPanel
{
	public SportsHud()
	{
		AddChild<GamemodeHudPanel>();
	}

	public override void OnDeleted()
	{
		DeleteChildren( true );

		base.OnDeleted();
	}
}
