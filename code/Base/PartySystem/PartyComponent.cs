using System.Collections.Generic;

namespace Sports.PartySystem;

public partial class PartyComponent : EntityComponent, ISingletonComponent
{
	/// <summary>
	/// Allow receiving party invites
	/// </summary>
	[ConVar.Client]
	public static bool PartyEnabled { get; set; } = true;

	public Client Client => Entity as Client;

	[Net] private Party party { get; set; }
	public Party Party
	{
		get => party; set
		{
			if ( party == value || Host.IsClient )
				return;
			if ( party.IsValid() )
			{
				party.LeaveParty( this );
				PartyManager.PartyChanged( To.Multiple( party.Members ) );
			}
			party = value;
			if ( value.IsValid() )
			{
				value.JoinParty( this );
				PartyManager.PartyChanged( To.Multiple( value.Members ) );
			}
		}
	}

	public IList<Client> Members => Party?.Members;

	public void Leave()
	{
		Party = null;
	}

	/// <summary>
	/// Got invited by another client
	/// </summary>
	public void Invited( Client cl )
	{
		UI.PartyLobby.OnInviteReceived( cl );
	}
}
