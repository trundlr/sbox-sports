namespace Sports;

[HammerEntity]
[Library( "sports_gamemode_orb" )]
[Title( "Gamemode Orb" )]
[Category( "Map Setup" )]
[Particle]
public partial class GamemodeOrb : GamemodeBaseTrigger
{
	public static HashSet<GamemodeOrb> Orbs { get; set; } = new();

	[Property( "effect_name", Title = "Particle System Name" ), ResourceType( "vpcf" )]
	public string ParticleSystemName { get; set; }

	protected TimeSince LastTouch = 1f;

	public override void Spawn()
	{
		base.Spawn();
		Orbs.Add( this );

		_ = new ParticleSystemEntity
		{
			Position = Position,
			ParticleSystemName = ParticleSystemName,
			StartActive = true
		};
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		Orbs.Remove( this );
	}

	public override void StartTouch( Entity other )
	{
		base.StartTouch( other );

		if ( LastTouch < 1f ) return;

		if ( other is PlazaPlayer player )
		{
			Log.Debug( $"Orb trying to add player: {player.Client.Name} to gamemode: {Gamemode}" );
			Gamemode?.AddClient( player.Client );
			LastTouch = 0;
		}
	}

	[Event.Tick.Server]
	private void ServerTick()
	{
		if ( !Debug.Enabled )
			return;

		DebugOverlay.Text( $"Gamemode Orb: {Gamemode}", Position );
	}
}
