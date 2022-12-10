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
		get => new Vector2( Style.Left.Value.Value / 100f * Screen.Width, Style.Top.Value.Value / 100 * Screen.Height );
		set
		{
			Style.Left = Length.Fraction( value.x / Screen.Width );
			Style.Top = Length.Fraction( value.y / Screen.Height );
		}
	}
	public float PixelSize => 40f / RockPaperScissors.Instance.Zoom * ScaleToScreen;
	public Icon Chasing { get; set; } = null;

	public Icon( IconType type )
	{
		SetType( type );
		Style.Left = Length.Fraction( Game.Random.Float( 1f ) );
		Style.Top = Length.Fraction( Game.Random.Float( 1f ) );
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

	public void PlaySound()
	{
		if ( Type == null ) return;

		var position = PixelPosition;
		Sound.FromScreen( $"sounds/{Type.ToString().ToLower()}.sound", position.x * ScaleFromScreen / Screen.Width, position.y * ScaleFromScreen / Screen.Width );
	}

	[Event.Client.Frame]
	void computeAI()
	{
		if ( Type == null ) return;

		Style.ZIndex = (int)PixelPosition.y; // Sort their ZIndex based on their vertical position to remove annoying clipping

		var velocity = Vector2.Zero;
		var currentPosition = PixelPosition;

		// Run from predators

		foreach ( var predator in Icon.All[Predator] )
		{
			var predatorPosition = predator.PixelPosition;

			if ( currentPosition.Distance( predatorPosition ) <= PixelSize * 2f )
			{
				velocity = (currentPosition - predatorPosition).Normal * Time.Delta * (40f / RockPaperScissors.Instance.Zoom) * ScaleToScreen;
				break;
			}
		}

		// Push same icons

		var pushOffset = Vector2.Zero;

		foreach ( var ally in Icon.All[Type.Value] )
		{
			if ( this == ally ) continue;

			var allyPosition = ally.PixelPosition;

			if ( currentPosition.Distance( allyPosition ) <= PixelSize * 1f )
			{
				pushOffset = (currentPosition - allyPosition).Normal * Time.Delta * (15f / RockPaperScissors.Instance.Zoom) * ScaleToScreen;
				break;
			}
		}

		// Chase preys

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
			velocity += (chasingPosition - currentPosition).Normal * Time.Delta * (50f / RockPaperScissors.Instance.Zoom) * ScaleToScreen;

			if ( chasingDistance <= PixelSize )
			{
				Chasing.SetType( Type.Value );
				PlaySound();
			}
		}

		// Apply new position
		var jiggle = new Vector2( Game.Random.Float( -1, 1 ), Game.Random.Float( -1, 1 ) ) * (0.5f / RockPaperScissors.Instance.Zoom) * Time.Delta * 60f * ScaleToScreen;
		PixelPosition += velocity.Clamp( 0.5f / RockPaperScissors.Instance.Zoom * Time.Delta * -60f, 0.5f / RockPaperScissors.Instance.Zoom * Time.Delta * 60f ) + jiggle + pushOffset;

	}
}
