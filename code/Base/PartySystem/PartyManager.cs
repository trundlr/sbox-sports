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

	/// <summary>
	/// List of all Parties
	/// </summary>
	[Net] public IList<Party> Parties { get; private set; }

	/// <summary>
	/// Create a Party for a PartyComponent
	/// </summary>
	/// <param name="comp"></param>
	public static void CreatePartyFor( PartyComponent comp )
	{
		if ( Host.IsClient )
			return;
		var party = new Party()
		{
			Host = comp.Client
		};
		Instance.Parties.Add( party );
		comp.Party = party;
		Log.Debug( "Created party " + party.NetworkIdent );
	}

	/// <summary>
	/// List All Party Members into the Console
	/// </summary>
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

	/// <summary>
	/// Join the Party of another Player directly
	/// </summary>
	/// <param name="otherPlayerNetID">The NetworkIdent of the Player Pawn</param>
	[ServerCmd]
	public static void JoinPlayer( int otherPlayerNetID )
	{
		if ( ConsoleSystem.Caller == null || Entity.FindByIndex( otherPlayerNetID )?.Client is not Client OtherPlayer )
			return;
		var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
		var otherComp = OtherPlayer.Components.GetOrCreate<PartyComponent>();
		if ( otherComp.Party == null ) // TODO: Make this system better, and not let everyone just join everybody
		{
			CreatePartyFor( otherComp );
		}
		comp.Party = otherComp.Party;
	}

	/// <summary>
	/// Send a Party Invite to another Player
	/// </summary>
	/// <param name="otherPlayerNetID">The NetworkIdent of the Player Pawn</param>
	[ServerCmd]
	public static void InvitePlayer( int otherPlayerNetID )
	{
		if ( ConsoleSystem.Caller == null || Entity.FindByIndex( otherPlayerNetID )?.Client is not Client OtherPlayer )
			return;
		var otherComp = OtherPlayer.Components.GetOrCreate<PartyComponent>();
		Log.Debug( $"{ConsoleSystem.Caller.Name} invited {OtherPlayer.Name} to a party" );
		if ( OtherPlayer.IsBot ) // no need to Invite Bots they should always accept
		{
			var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
			if ( !comp.Party.IsValid() )
			{
				CreatePartyFor( comp );
			}
			otherComp.Party = comp.Party;
			return;
		}
		if ( !otherComp.Party.IsValid() )
			RpcInvitePlayer( To.Single( OtherPlayer ), ConsoleSystem.Caller.NetworkIdent );
	}

	[ClientRpc]
	public static void RpcInvitePlayer( int fromPlayerNetID )
	{
		if ( Client.All.FirstOrDefault( e => e.NetworkIdent == fromPlayerNetID ) is not Client OtherPlayer )
			return;
		if ( Local.Client.IsBot )
		{
			JoinPlayer( fromPlayerNetID );
			return;
		}
		Local.Client.Components.Get<PartyComponent>().Invited( OtherPlayer );
	}

	/// <summary>
	/// Leave your Current Party. TODO: Add a UI button for it instead of just a console command
	/// </summary>
	[ServerCmd]
	public static void LeaveParty()
	{
		if ( ConsoleSystem.Caller == null )
			return;
		var comp = ConsoleSystem.Caller.Components.GetOrCreate<PartyComponent>();
		comp.Party = null;
	}

	/// <summary>
	/// Notify that the Party members have changed
	/// </summary>
	[ClientRpc]
	public static void PartyChanged()
	{
		UI.PartyLobby.OnPartyChanged();
	}
}
