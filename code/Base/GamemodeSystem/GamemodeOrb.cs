namespace Sports;

[Library( "sports_gamemode_orb" )]
[EditorModel( "models/dev/gamemode_orb.vmdl" )]
[Display( Name = "Gamemode Orb" )]
[Category( "Map Setup" )]
[Sphere( 128f, 0, 125, 255 )]
public partial class GamemodeOrb : AnimEntity
{
	public static HashSet<GamemodeOrb> Orbs { get; set; }

	[Property]
	public string GamemodeId { get; set; }

	public BaseGamemode LinkedGamemode { get; set; }

	protected TimeSince LastTouch = 1f;

	[Event.Entity.PostSpawn]
	protected void PostEntitiesSpawned()
	{
		LinkedGamemode = SportsGame.Instance?.GetGamemodeFromId( GamemodeId );
		Log.Info( "Orb: Linked to gamemode." );
	}

	public override void Spawn()
	{
		base.Spawn();
		Orbs.Add( this );
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
