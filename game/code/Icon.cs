namespace RockPaperScissors;

public enum IconType
{
	Rock,
	Paper,
	Scissors
}

public partial class Icon : Label
{
	public IconType? Type;
	public static Dictionary<IconType, List<Icon>> All = new()
	{
		{ IconType.Rock, new() },
		{ IconType.Paper, new() },
		{ IconType.Scissors, new() }
	};
	public IconType Prey => (IconType)((int)(Type.Value - 1) == -1 ? Enum.GetValues( typeof( IconType ) ).Length - 1 : (int)(Type.Value - 1)); // The type before is what this icon can eat - Loops back
	public IconType Predator => (IconType)((int)(Type.Value + 1) % Enum.GetValues( typeof( IconType ) ).Length); // The type after is what this icon gets eaten by - Loops back

	public Vector2 PixelPosition
	{
		get => new Vector2( Style.Left.Value.GetPixels( ScaleToScreen ) * Screen.Width, Style.Top.Value.GetPixels( ScaleToScreen ) * Screen.Height );
		set
		{
			Style.Left = Length.Fraction( value.x * ScaleFromScreen / Screen.Width );
			Style.Top = Length.Fraction( value.y * ScaleFromScreen / Screen.Height );
		}
	}
	public float PixelSize => 60f; // Style.Height is always null?

	public Icon( IconType type )
	{
		SetType( type );
		Style.Left = Length.Fraction( Rand.Float( 1f ) );
		Style.Top = Length.Fraction( Rand.Float( 1f ) );
	}

	public void SetType( IconType type )
	{
		if ( Type != null )
			All[Type.Value].Remove( this );
		Type = type;
		All[type].Add( this );

		Text = Type switch
		{
			IconType.Rock => "🗿",
			IconType.Paper => "📜",
			IconType.Scissors => "✂️",
			_ => String.Empty
		};
	}

	[Event.Client.Frame]
	void computeAI()
	{
		if ( Type == null ) return;

		Style.ZIndex = (int)PixelPosition.y; // Sort their ZIndex based on their vertical position to remove annoying clipping

		foreach ( var predator in Icon.All[Predator] )
		{
			if ( PixelPosition.Distance( predator.PixelPosition ) <= PixelSize )
			{
				SetType( Predator );
				break;
			}
		}

		var allPreys = Icon.All[Prey];

		foreach ( var prey in allPreys )
		{
			if ( PixelPosition.Distance( prey.PixelPosition ) <= PixelSize )
			{
				prey.SetType( Type.Value );
				break;
			}
		}

	}
}
