namespace Sports.PartySystem;

public partial class Party : Entity // Use Entity for Parties since BaseNetworkables don't like Lists currently
{
	[Net] private Client _partyHost { get; set; }
	public Client Host
	{
		get => _partyHost; set
		{
			if ( _partyHost == null )
				_partyHost = value;
		}
	}
	[Net] public IList<Client> Members { get; set; } // Can't store the PartyComponent Crashes game when adding to list directly after creation

	/// <summary>
	/// kick a Client from the Party
	/// </summary>
	/// <param name="clientNetworkID"></param>
	[ServerCmd]
	public static void KickPlayer( int clientNetworkID )
	{
		if ( ConsoleSystem.Caller is not Client caller )
			return;
		var callerComp = caller.Components.Get<PartyComponent>();
		if ( callerComp?.Party?.Host != caller )
		{
			Log.Debug( "Only the party host can kick players" );
			Log.Debug( "Party host: " + callerComp?.Party?.Host?.Name );
			return;
		}
		if ( callerComp?.Party?.Members.FirstOrDefault( e => e.NetworkIdent == clientNetworkID )?.Components.Get<PartyComponent>( true ) is PartyComponent comp )
		{
			comp.Leave();
		}
	}

	/// <summary>
	/// Accept an Invite
	/// </summary>
	/// <param name="clientNetworkID"></param>
	[ServerCmd]
	public static void AcceptInvite( int clientNetworkID )
	{
		if ( ConsoleSystem.Caller is not Client caller )
			return;
		var callerComp = caller.Components.Get<PartyComponent>();
		if ( Client.All.FirstOrDefault( e => e.NetworkIdent == clientNetworkID )?.Components.Get<PartyComponent>( true ) is PartyComponent comp )
		{
			if ( comp.Party == null )
			{
				PartyManager.CreatePartyFor( comp );
			}
			callerComp.Party = comp.Party;
		}
	}

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	/// <summary>
	/// Leave this Party
	/// </summary>
	/// <param name="comp"></param>
	public void LeaveParty( PartyComponent comp )
	{
		if ( IsClient )
		{
			LeaveParty();
		}
		if ( Members.Contains( comp.Client ) )
		{
			if ( comp.Client == _partyHost )
			{
				_partyHost = Members.FirstOrDefault( e => e != comp.Client );
			}
			Members.Remove( comp.Client );
			if ( Members.Count <= 1 )
			{
				PartyManager.Instance.Parties.Remove( this );
				Delete();
			}
		}
	}

	[ServerCmd]
	public static void LeaveParty()
	{
		if ( ConsoleSystem.Caller is not Client caller )
			return;
		var callerComp = caller.Components.Get<PartyComponent>();
		callerComp?.Party?.LeaveParty( callerComp );
	}

	/// <summary>
	/// Join this Party
	/// </summary>
	/// <param name="comp"></param>
	public void JoinParty( PartyComponent comp )
	{
		if ( !Members.Contains( comp.Client ) )
		{
			Members.Add( comp.Client );
		}
	}
}
