
namespace Sports.UI;

[UseTemplate, Library( "ControlGlyph", Alias = new[] { "ButtonHint" } )]
public class ControlGlyph : Panel
{

	public Image Glyph { get; set; }
	public Label Text { get; set; }
	public InputButton Button { get; set; }
	public InputGlyphSize Size { get; set; } = InputGlyphSize.Small;

	GlyphStyle style = default;
	private bool solid = false;
	private bool neutral = false;

	private static Texture UnboundTexture = Texture.Load( FileSystem.Mounted, "/ui/unbound.png" );

	public ControlGlyph()
	{
		AddClass( "control-glyph" );
	}

	protected override void PreTemplateApplied()
	{
		base.PreTemplateApplied();
		solid = false;
		neutral = false;
	}

	public override void SetProperty( string name, string value )
	{
		switch ( name )
		{
			case "glyph":
				if ( Enum.TryParse( value.ToTitleCase().Replace( " ", "" ), out InputButton btn ) )
				{
					Button = btn;
					SetGlyph( btn );
				}
				break;
			case "glyphsize":
				if ( Enum.TryParse( value.ToTitleCase(), out InputGlyphSize size ) )
				{
					Size = size;
				}
				break;
			case "glyphstyle":
				if ( value.ToLower() == "dark" )
				{
					style = GlyphStyle.Dark;
				}
				else if ( value.ToLower() == "light" )
				{
					style = GlyphStyle.Light;
				}
				break;
			case "solid":
				solid = true;
				break;
			case "neutral":
				neutral = true;
				break;
			default:
				base.SetProperty( name, value );
				break;
		}
	}

	public override void SetContent( string value )
	{
		base.SetContent( value );
		Text.Text = value;
	}

	public void SetGlyph( InputButton btn, InputGlyphSize size = InputGlyphSize.Small )
	{
		var Style = style;
		if ( solid )
			Style = Style.WithSolidABXY();
		if ( neutral )
			Style = Style.WithNeutralColorABXY();

		Texture glyphimage = Input.GetGlyph( btn, size, Style );
		if ( glyphimage == null || string.IsNullOrEmpty( Input.GetButtonOrigin( btn ) ) )
		{
			glyphimage = UnboundTexture;
			Text.SetClass( "hide", Text.TextLength == 0 );
			Glyph.Texture = glyphimage;
			Glyph.Style.Width = 32;
			Glyph.Style.Height = 32;
			return;
		}
		Glyph.SetClass( "medium", size == InputGlyphSize.Medium );
		Glyph.SetClass( "large", size == InputGlyphSize.Large );
		Text.SetClass( "hide", Text.TextLength == 0 );
		Glyph.Texture = glyphimage;
		Glyph.Style.AspectRatio = (float)glyphimage.Width / glyphimage.Height;
	}

	public override void Tick()
	{
		base.Tick();
		SetGlyph( Button, Size );
	}


}
