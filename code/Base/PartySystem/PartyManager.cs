using System.Collections.Generic;

namespace Sports.PartySystem;

public partial class PartyManager : Entity
{
	public static PartyManager Instance { get; protected set; }

	public override void Spawn()
	{
		base.Spawn();
		Transmit = TransmitType.Always;
		Instance = this;
	}

	/// <summary>
	/// List of all parties
	/// </summary>
	[Net]
	public IList<Party> Parties { get; private set; }

	/// <summary>
	/// Create a party for a PartyComponent
	/// </summary>
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
	/// List all party members into the console
	/// </summary>
	[ConCmd.Server]
	public static void ListPartyMembers()
	{
		if ( ConsoleSystem.Caller == null )
			return;

		var comp = ConsoleSystem.Caller.GetPartyComponent();
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
	/// Join the party of another player directly
	/// </summary>
	/// <param name="otherPlayerNetID">The networkIdent of the player pawn</param>
	[ConCmd.Server]
	public static void JoinPlayer( int otherPlayerNetID )
	{
		if ( ConsoleSystem.Caller == null || Entity.FindByIndex( otherPlayerNetID )?.Client is not Client OtherPlayer )
			return;

		var comp = ConsoleSystem.Caller.GetPartyComponent();
		var otherComp = OtherPlayer.GetPartyComponent();
		if ( otherComp.Party == null ) // TODO: Make this system better, and not let everyone just join everybody
		{
			CreatePartyFor( otherComp );
		}

		comp.Party = otherComp.Party;
	}

	/// <summary>
	/// Send a party invite to another player
	/// </summary>
	/// <param name="otherPlayerNetID">The networkIdent of the player pawn</param>
	[ConCmd.Server]
	public static void InvitePlayer( int otherPlayerNetID )
	{
		if ( ConsoleSystem.Caller == null || Entity.FindByIndex( otherPlayerNetID )?.Client is not Client OtherPlayer )
			return;

		var otherComp = OtherPlayer.GetPartyComponent();

		Log.Debug( $"{ConsoleSystem.Caller.Name} invited {OtherPlayer.Name} to a party" );

		if ( OtherPlayer.IsBot ) // no need to Invite Bots they should always accept
		{
			var comp = ConsoleSystem.Caller.GetPartyComponent();
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

	/// <summary>
	/// Test invite yourself to a party of a bot. Doesn't work if no bots are present.
	/// </summary>
	[ConCmd.Server]
	public static void TestInvitePlayer()
	{
		if ( ConsoleSystem.Caller == null || Client.All.FirstOrDefault( e => e.IsBot ) is not Client OtherPlayer )
			return;
		RpcInvitePlayer( To.Single( ConsoleSystem.Caller ), OtherPlayer.NetworkIdent );
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

		Local.Client.GetPartyComponent()?.Invited( OtherPlayer );
	}

	/// <summary>
	/// Leave your current party
	/// </summary>
	[ConCmd.Server]
	public static void LeaveParty()
	{
		if ( ConsoleSystem.Caller == null )
			return;

		var comp = ConsoleSystem.Caller.GetPartyComponent();
		comp.Party = null;
	}

	/// <summary>
	/// Notify that party members have changed
	/// </summary>
	[ClientRpc]
	public static void PartyChanged()
	{
		UI.PartyLobby.OnPartyChanged();
	}
}
