using System.Collections.Generic;

namespace Sports.PartySystem;

public partial class PartyComponent : EntityComponent, ISingletonComponent
{

	[ClientVar( "allow_party_invites" )]
	public static bool PartyEnabled { get; set; } = true;

	public Client Client => Entity as Client;

	[Net] private Party _party { get; set; }
	public Party Party
	{
		get => _party; set
		{
			if ( _party == value || Host.IsClient )
				return;
			if ( _party.IsValid() )
			{
				_party.LeaveParty( this );
				PartyManager.PartyChanged( To.Multiple( _party.Members ) );
			}
			_party = value;
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
	/// Got invited by another Client
	/// </summary>
	/// <param name="cl"></param>
	public void Invited( Client cl )
	{
		UI.PartyLobby.OnInviteReceived( cl );
	}
}
