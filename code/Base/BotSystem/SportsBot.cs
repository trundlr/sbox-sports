namespace Sports.BotSystem;

public class SportsBot : Bot
{
	protected BaseBotBehaviour behaviour;

	public override void BuildInput( InputBuilder builder )
	{
		builder.Clear();

		if ( behaviour != null )
		{
			builder.InputDirection = behaviour.InputDirection;
			builder.ViewAngles = behaviour.ViewAngles;
			builder.SetButton( InputButton.PrimaryAttack, behaviour.Attack1 );
			builder.SetButton( InputButton.SecondaryAttack, behaviour.Attack2 );
		}
	}

	public virtual void ApplyBehaviour<T>() where T : BaseBotBehaviour, new()
	{
		Log.Debug( $"Bot behaviour applied | {typeof( T )}" );
		behaviour = new T
		{
			Bot = this
		};
	}

	public override void Tick()
	{
		behaviour?.Tick();
	}
}
