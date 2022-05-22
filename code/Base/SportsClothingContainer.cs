using System.Text.Json.Serialization;

namespace Sports;

/// <summary>
/// Holds a collection of clothing items. Won't let you add items that aren't compatible.
/// </summary>
public class SportsClothingContainer
{
	public List<Clothing> Clothing = new();
	public List<Clothing.ClothingCategory> RestrictedClothingCategories = new();

	/// <summary>
	/// Add a clothing item if we don't already contain it, else remove it
	/// </summary>
	public void Toggle( Clothing clothing )
	{
		if ( Has( clothing ) ) Remove( clothing );
		else Add( clothing );
	}

	/// <summary>
	/// Add clothing item
	/// </summary>
	public void Add( Clothing clothing )
	{
		if ( clothing is null )
		{
			Log.Warning( "Null resource; possibly in need of a manual compile." );
			return;
		}

		Clothing.RemoveAll( x => !x.CanBeWornWith( clothing ) );
		Clothing.Add( clothing );
	}

	/// <summary>
	/// Add a range of clothing items
	/// </summary>
	/// <param name="clothes"></param>
	public void AddRange( Clothing[] clothes )
	{
		foreach ( var clothingItem in clothes )
			Add( clothingItem );
	}

	/// <summary>
	/// Load the clothing from this client's data. This is a different entry
	/// point than just calling Deserialize directly because if we have
	/// inventory based skins at some point, we can validate ownership here
	/// </summary>
	public void LoadFromClient( Client cl )
	{
		var data = cl.GetClientData( "avatar" );
		Deserialize( data );
	}

	/// <summary>
	/// Remove clothing item
	/// </summary>
	public void Remove( Clothing clothing )
	{
		Clothing.Remove( clothing );
	}

	/// <summary>
	/// Remove a range of clothing items
	/// </summary>
	/// <param name="clothes"></param>
	public void RemoveRange( Clothing[] clothes )
	{
		foreach ( var clothingItem in clothes )
		{
			if ( !Has( clothingItem ) )
				return;

			Remove( clothingItem );
		}
	}

	/// <summary>
	/// Remove a specific category of clothing, useful for things like boxing where you want the player to be topless.
	/// </summary>
	/// <param name="categorytoRestrict"></param>
	public void RestrictAllOfCategory( Clothing.ClothingCategory categorytoRestrict )
	{
		if ( !RestrictedClothingCategories.Contains( categorytoRestrict ) )
			RestrictedClothingCategories.Add( categorytoRestrict );
	}

	/// <summary>
	/// Returns true if we have this clothing item
	/// </summary>
	public bool Has( Clothing clothing ) => Clothing.Contains( clothing );

	/// <summary>
	/// Return a list of bodygroups and what their value shold be
	/// </summary>
	public IEnumerable<(string name, int value)> GetBodyGroups()
	{
		var mask = Clothing.Select( x => x.HideBody ).DefaultIfEmpty().Aggregate( ( a, b ) => a | b );

		yield return ("head", (mask & Sandbox.Clothing.BodyGroups.Head) != 0 ? 1 : 0);
		yield return ("Chest", (mask & Sandbox.Clothing.BodyGroups.Chest) != 0 ? 1 : 0);
		yield return ("Legs", (mask & Sandbox.Clothing.BodyGroups.Legs) != 0 ? 1 : 0);
		yield return ("Hands", (mask & Sandbox.Clothing.BodyGroups.Hands) != 0 ? 1 : 0);
		yield return ("Feet", (mask & Sandbox.Clothing.BodyGroups.Feet) != 0 ? 1 : 0);
	}

	/// <summary>
	/// Serialize to Json
	/// </summary>
	public string Serialize()
	{
		return System.Text.Json.JsonSerializer.Serialize( Clothing.Select( x => new Entry { Id = x.ResourceId } ) );
	}

	/// <summary>
	/// Deserialize from Json
	/// </summary>
	public void Deserialize( string json )
	{
		Clothing.Clear();

		if ( string.IsNullOrWhiteSpace( json ) )
			return;

		try
		{
			var entries = System.Text.Json.JsonSerializer.Deserialize<Entry[]>( json );

			foreach ( var entry in entries )
			{
				var item = ResourceLibrary.Get<Clothing>( entry.Id );

				if ( item == null ) continue;
				Add( item );
			}
		}
		catch ( System.Exception e )
		{
			Log.Warning( e, "Error deserailizing clothing" );
		}
	}

	/// <summary>
	/// Used for serialization
	/// </summary>
	public struct Entry
	{
		[JsonPropertyName( "id" )]
		public int Id { get; set; }

		// in the future we could allow some
		// configuration (tint etc) of items
		// which is why this is a struct instead
		// of serializing an array of ints
	}

	List<AnimatedEntity> ClothingModels = new();

	public void ClearEntities()
	{
		foreach ( var model in ClothingModels )
		{
			model.Delete();
		}
		ClothingModels.Clear();
	}

	/// <summary>
	/// Dress this citizen with clothes defined inside this class. We'll save the created entities in ClothingModels.
	/// All clothing entities are tagged with "clothes".
	/// </summary>
	public void DressEntity( AnimatedEntity citizen, bool hideInFirstPerson = true, bool castShadowsInFirstPerson = true )
	{
		//
		// Start with defaults
		//
		citizen.SetMaterialGroup( "default" );

		//
		// Remove old models
		//
		ClearEntities();

		//
		// Create clothes models
		//
		foreach ( var c in Clothing )
		{
			// Ignore any clothing items that are currently prohibited
			if ( RestrictedClothingCategories.Contains( c.Category ) )
				continue;

			if ( c.Model == "models/citizen/citizen.vmdl" )
			{
				citizen.SetMaterialGroup( c.MaterialGroup );
				continue;
			}

			var anim = new AnimatedEntity( c.Model, citizen );

			anim.Tags.Add( "clothes" );

			anim.EnableHideInFirstPerson = hideInFirstPerson;
			anim.EnableShadowInFirstPerson = castShadowsInFirstPerson;

			if ( !string.IsNullOrEmpty( c.MaterialGroup ) )
				anim.SetMaterialGroup( c.MaterialGroup );

			ClothingModels.Add( anim );
		}

		//
		// Set body groups
		//
		foreach ( var group in GetBodyGroups() )
		{
			citizen.SetBodyGroup( group.name, group.value );
		}
	}
}
