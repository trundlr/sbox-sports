namespace Sports;

[Library( "sports_gamemode_orb" )]
[Display( Name = "Gamemode Orb" )]
[Category( "Map Setup" )]
[Particle( "orbparticlepath" )]
public partial class GamemodeOrb : BaseTrigger
{
	public static HashSet<GamemodeOrb> Orbs { get; set; } = new();

	[Property, ResourceType( "vpcf" )]
	private string orbParticlePath { get; set; } = "particles/gamemode_orb/orb.vpcf";
	private ParticleSystemEntity orbParticle { get; set; }
	[Property( "snapshot_file" ), ResourceType( "vsnap" )]
	public string SnapshotFile { get; set; }
	[Property( "snapshot_mesh" ), FGDType( "node_id" )]
	private string SnapshotMesh { get; set; }

	[Property]
	public string GamemodeId { get; set; }

	public BaseGamemode LinkedGamemode { get; set; }

	protected TimeSince LastTouch = 1f;

	[Event.Entity.PostSpawn]
	protected void PostEntitiesSpawned()
	{
		LinkedGamemode = SportsGame.Instance?.GetGamemodeFromId( GamemodeId );
		Log.Debug( "Orb: Linked to gamemode." );
	}

	public override void Spawn()
	{
		base.Spawn();
		Orbs.Add( this );

		Transmit = TransmitType.Always;

		SetupPhysicsFromSphere( PhysicsMotionType.Keyframed, Vector3.Zero, 64f );
		CollisionGroup = CollisionGroup.Trigger;

		orbParticle = new ParticleSystemEntity
		{
			Transform = Transform,
			SnapshotFile = SnapshotFile,
			ParticleSystemName = orbParticlePath,
			Parent = this,
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
			LinkedGamemode?.AddClient( player.Client );
			LastTouch = 0;
		}
	}
}
