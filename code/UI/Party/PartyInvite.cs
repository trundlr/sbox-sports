using System;
using Sports.PartySystem;

namespace Sports.UI;

[UseTemplate]
public class PartyInvite : Panel
{
	public Label InviteText { get; set; }
	private Panel ProgressBar { get; set; }
	public Client Client { get; set; }

	private TimeSince received;
	public float TimeToAccept { get; set; } = 3;

	public PartyInvite()
	{
		received = 0;
	}

	public override void Tick()
	{
		base.Tick();
		if ( Local.Client.IsSameParty( Client ) )
		{
			Delete();
			return;
		}
		if ( !((PseudoClass & PseudoClass.FirstChild) != 0) )
		{
			ProgressBar.Parent?.AddClass( "hide" );
			received = 0;
			return;
		}
		else if ( Input.Pressed( InputButton.Use ) )
		{
			PartyLobby.AcceptedInvite( Client.NetworkIdent );
			return;
		}
		else if ( Input.Pressed( InputButton.Menu ) )
		{
			Delete();
			return;
		}

		ProgressBar.Parent?.RemoveClass( "hide" );
		ProgressBar.Style.Width = Length.Fraction( received / TimeToAccept );

		if ( received > TimeToAccept )
		{
			Delete();
		}
	}
}
