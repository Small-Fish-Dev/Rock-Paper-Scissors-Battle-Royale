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
	public float PixelSize => 40f; // Style.Height is always null?
	public Icon Chasing { get; set; } = null;

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

		var currentPosition = PixelPosition;

		foreach ( var predator in Icon.All[Predator] )
		{
			if ( currentPosition.Distance( predator.PixelPosition ) <= PixelSize )
			{
				SetType( Predator );
				break;
			}
		}

		var chasingDistance = 0f;
		var chasingPosition = Vector2.Zero;

		if ( Chasing != null )
		{
			if ( Chasing.Type.Value != Prey )
				Chasing = null;
			else
			{
				chasingPosition = Chasing.PixelPosition;
				chasingDistance = currentPosition.Distance( chasingPosition );
			}
		}
			

		foreach ( var prey in Icon.All[Prey] )
		{
			var preyPosition = prey.PixelPosition;
			var preyDistance = currentPosition.Distance( preyPosition );
			if ( Chasing != null )
			{
				if ( preyDistance < chasingDistance )
				{
					Chasing = prey;
					chasingDistance = preyDistance;
					chasingPosition = preyPosition;
				}
			}
			else
			{
				Chasing = prey;
				chasingDistance = preyDistance;
				chasingPosition = preyPosition;
			}
		}

		if ( Chasing != null )
		{
			PixelPosition += (chasingPosition - currentPosition).Normal * Time.Delta * 50f;

			if ( chasingDistance <= PixelSize )
			{
				Chasing.SetType( Type.Value );
			}
		}

	}
}
