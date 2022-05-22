namespace Sports;

public abstract partial class Interaction
{
	/// <summary>
	/// Who is executing this interaction
	/// </summary>
	public Entity Owner { get; set; }

	/// <summary>
	/// Network Interaction
	/// </summary>
	public virtual string ID => "";

	/// <summary>
	/// What we'll call the interaction on the client
	/// </summary>
	public virtual string NiceName => "";

	/// <summary>
	/// Called before requesting the interaction serverside, so we can onmit it entirely.
	/// </summary>
	/// <returns></returns>
	public virtual bool ResolveOnServer => true;

	/// <summary>
	/// Decides if we can even resolve in the first place. Omitted in UI if false.
	/// </summary>
	/// <returns></returns>
	public virtual bool CanResolve() => true;

	public void ClientResolve()
	{
		OnClientResolve();
	}
	public void ServerResolve()
	{
		OnServerResolve();
	}

	/// <summary>
	/// Called when an interaction is resolved clientside
	/// </summary>
	protected virtual void OnClientResolve() { }

	/// <summary>
	/// Called when an interaction is resolved serverside
	/// </summary>
	protected virtual void OnServerResolve() { }

	[ConCmd.Server( "sports_interact" )]
	public static void TryServerResolve( int netIdent, string id )
	{
		var entity = Entity.FindByIndex( netIdent );
		if ( entity.IsValid() && entity is IInteractable interactable )
		{
			var options = interactable.GetInteractions();
			var interaction = options.FirstOrDefault( x => x.ID == id );

			if ( interaction is null )
				return;

			interaction.ServerResolve();
		}
	}
}
