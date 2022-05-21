namespace Sports;

public abstract partial class Interaction : Entity
{
	[Net] public virtual string InteractionName => "";

	public void Resolve()
	{
		OnResolve();
	}

	protected virtual void OnResolve() { }
}
