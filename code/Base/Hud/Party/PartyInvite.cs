using System;
using Sports.PartySystem;

namespace Sports.UI;

[UseTemplate]
public class PartyInvite : Panel
{
	public Label InviteText { get; internal set; }
	private Panel ProgressBar { get; set; }
	public Client Client { get; internal set; }

	TimeSince _received;
	public float TimeToAccept { get; set; } = 3;

	public PartyInvite()
	{
		_received = 0;
	}

	public override void Tick()
	{
		base.Tick();
		if ( !((PseudoClass & PseudoClass.FirstChild) != 0) )
		{
			ProgressBar.Parent?.AddClass( "hide" );
			_received = 0;
			return;
		}
		ProgressBar.Parent?.RemoveClass( "hide" );
		ProgressBar.Style.Width = Length.Fraction( _received / TimeToAccept );

		if ( _received > TimeToAccept )
		{
			Delete();
		}
	}
}
