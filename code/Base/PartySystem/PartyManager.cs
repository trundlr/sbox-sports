using System.Collections.Generic;

namespace Sports.PartySystem;

public partial class PartyManager : Entity
{
	private static PartyManager _instance;
	public static PartyManager Instance
	{
		get
		{
			return _instance;
		}
	}
	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
		_instance = this;
	}

	[Net] public IList<Party> Parties { get; private set; }
	private static void CreatePartyFor( PartyComponent comp )
	{
		if ( Host.IsClient )
			return;
		var party = new Party();
		Instance.Parties.Add( party );
		comp.Party = party;
		Log.Debug( "Created party " + party.NetworkIdent );
	}
	[ServerCmd]
	public static void ListPartyMembers()
	{
		if ( ConsoleSystem.Caller == null )
			return;
		var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
		if ( comp.Party == null )
		{
			Log.Debug( "Party not found" );
			return;
		}

		Log.Debug( $"Party {comp.Party.NetworkIdent} members: " );
		foreach ( var member in comp.Party.Members )
		{
			Log.Debug( "  " + member.Name );
		}
	}
	[ServerCmd]
	public static void JoinPlayer( int OtherPlayerNetID )
	{
		if ( ConsoleSystem.Caller == null || Entity.FindByIndex( OtherPlayerNetID )?.Client is not Client OtherPlayer )
			return;
		var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
		var otherComp = OtherPlayer.Components.GetOrCreate<PartyComponent>();
		if ( otherComp.Party == null ) // TODO: Make this system better, and not let everyone just join everybody
		{
			CreatePartyFor( otherComp );
		}
		comp.Party = otherComp.Party;
	}
	[ServerCmd]
	public static void InvitePlayer( int OtherPlayerNetID )
	{
		if ( ConsoleSystem.Caller == null || Entity.FindByIndex( OtherPlayerNetID )?.Client is not Client OtherPlayer )
			return;
		var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
		var otherComp = OtherPlayer.Components.GetOrCreate<PartyComponent>();
		if ( comp.Party == null ) // TODO: Make this system better, and not let everyone just join everybody
		{
			CreatePartyFor( comp );
		}
		otherComp.Party = comp.Party;
	}
	[ServerCmd]
	public static void SendTestInvite()
	{
		if ( ConsoleSystem.Caller == null )
			return;
		RpcInviteReceived( To.Single( ConsoleSystem.Caller ) );
	}
	[ClientRpc]
	public static void RpcInviteReceived()
	{
		var comp = Local.Client.Components.Get<PartyComponent>();
		comp?.Invite();
	}
	[ServerCmd]
	public static void LeaveParty()
	{
		if ( ConsoleSystem.Caller == null )
			return;
		var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
		comp.Party = null;
	}


	[ClientRpc]
	public static void PartyChanged()
	{
		UI.PartyLobby.OnPartyChanged();
	}
}
