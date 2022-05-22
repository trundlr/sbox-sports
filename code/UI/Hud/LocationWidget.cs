namespace Sports.UI;

[UseTemplate]
public partial class LocationWidget : Panel
{
	public string CurrentArea
	{
		get
		{
			var game = Local.Client.GetGamemode();
			if ( !game.IsValid() )
				return "Plaza";

			return game.Name;
		}
	}
}
