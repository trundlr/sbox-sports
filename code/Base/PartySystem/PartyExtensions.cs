using Sports.PartySystem;

namespace Sports;

public static class PartyExtensions
{

	/// <summary>
	/// Get the party of the client
	/// </summary>
	public static Party GetParty( this Client cl )
	{
		return GetPartyComponent( cl )?.Party;
	}

	/// <summary>
	/// Get the party component of the client
	/// </summary>
	public static PartyComponent GetPartyComponent( this Client cl )
	{
		if ( Host.IsServer )
			return cl.Components.GetOrCreate<PartyComponent>();
		return cl.Components.Get<PartyComponent>();
	}

	/// <summary>
	/// Check if two clients are in the same party
	/// </summary>
	public static bool IsSameParty( this Client cl, Client other )
	{
		return cl.GetParty() == other.GetParty() && cl.GetParty().IsValid();
	}

	/// <summary>
	/// Get party member by network id
	/// </summary>
	public static PartyComponent GetPartyMember( this Client cl, int clientNetworkID )
	{
		return cl.GetParty()?.Members.FirstOrDefault( e => e.NetworkIdent == clientNetworkID )?.GetPartyComponent();
	}
}
