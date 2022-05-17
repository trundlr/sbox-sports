namespace Sports.PartySystem;

public partial class Party : Entity // Use Entity for Parties since BaseNetworkables don't like Lists currently
{
	[Net] public IList<Client> Members { get; set; } // Can't store the PartyComponent Crashes game when adding to list directly after creation

	[ServerCmd]
	public static void KickPlayer( int ClientNetworkID )
	{
		Log.Info( "KickPlayer" );
		if ( ConsoleSystem.Caller is not Client caller )
			return;
		Log.Info( "Caller is " + caller.Name );
		var callerComp = caller.Components.Get<PartyComponent>();
		if ( callerComp?.Party?.Members.FirstOrDefault( e => e.NetworkIdent == ClientNetworkID )?.Components.Get<PartyComponent>( true ) is PartyComponent comp )
		{
			Log.Info( "Found party member" );
			comp.Leave();
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	public void LeaveParty( PartyComponent Comp )
	{
		if ( Members.Contains( Comp.Client ) )
		{
			Members.Remove( Comp.Client );
			if ( Members.Count <= 1 )
			{
				PartyManager.Instance.Parties.Remove( this );
				Delete();
			}
		}
	}
	public void JoinParty( PartyComponent Comp )
	{
		if ( !Members.Contains( Comp.Client ) )
		{
			Members.Add( Comp.Client );
		}
	}
}
