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

	[Net]
	private Party _Party { get; set; }

	public Party Party
	{
		get => _Party; set
		{
			if ( _Party == value || Host.IsClient )
				return;
			if ( _Party.IsValid() )
			{
				_Party.LeaveParty( this );
				PartyManager.PartyChanged( To.Multiple( _Party.Members ) );
			}
			_Party = value;
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
