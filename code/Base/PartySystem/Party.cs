namespace Sports.PartySystem;

public partial class Party : Entity // Use Entity for Parties since BaseNetworkables don't like Lists currently
{
	[Net]
	private Client _Host { get; set; }

	public Client Host
	{
		get => _Host; set
		{
			if ( _Host == null )
				_Host = value;
		}
	}
	[Net]
	public IList<Client> Members { get; set; } // Can't store the PartyComponent Crashes game when adding to list directly after creation

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
	}

	/// <summary>
	/// Leave this party
	/// </summary>
	public void LeaveParty( PartyComponent comp )
	{
		if ( IsClient )
		{
			PartyManager.LeaveParty();
		}
		if ( Members.Contains( comp.Client ) )
		{
			if ( comp.Client == _Host )
			{
				_Host = Members.FirstOrDefault( e => e != comp.Client );
			}
			Members.Remove( comp.Client );
			if ( Members.Count <= 1 )
			{
				PartyManager.Instance.Parties.Remove( this );
				Delete();
			}
		}
	}

	/// <summary>
	/// Join this party
	/// </summary>
	public void JoinParty( PartyComponent comp )
	{
		if ( !Members.Contains( comp.Client ) )
		{
			Members.Add( comp.Client );
		}
	}
}
