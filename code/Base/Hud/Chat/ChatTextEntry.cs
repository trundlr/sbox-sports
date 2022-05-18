namespace Sports.UI;

public class PartyChatTextEntry : TextEntry
{
	public override void OnButtonTyped( string button, KeyModifiers km )
	{
		base.OnButtonTyped( button, km );
		if ( button == "tab" )
		{
			CreateEvent( "onchatswitch" );
		}
	}
}
