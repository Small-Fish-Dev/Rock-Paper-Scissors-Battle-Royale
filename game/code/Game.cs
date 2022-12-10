global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;
global using System;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;
global using System.Collections.Generic;

namespace RockPaperScissors;

public enum GameState
{
	Paused,
	Playing,
	Ending
}

public partial class RockPaperScissors : GameManager
{
	public static RockPaperScissors Instance;
	internal GameState state { get; set; } = GameState.Paused;
	public GameState State
	{
		get => state;
		set
		{
			state = value;
			Event.Run( "StateChange", value );
		}
	}
	public float Zoom = 1f;
	public IconType? Winner = null;

	public RockPaperScissors()
	{
		Instance = this;
		if ( Game.IsClient )
			new UI();
	}

	[Event.Tick.Client]
	void GameLogic()
	{
		if ( Time.Tick % 60 != 0 ) return; // Check once in a while :-)

		if ( State == GameState.Ending )
		{
			Zoom += 0.1f;
			State = GameState.Playing;
			Winner = null;
		}

		if ( State == GameState.Playing )
		{
			var winner = CheckWinner();

			if ( winner != null )
			{
				State = GameState.Ending;
				Winner = winner;
			}
		}

		if ( State == GameState.Paused )
			State = GameState.Playing;
	}

	public IconType? CheckWinner()
	{
		var allTypes = (IconType[])Enum.GetValues( typeof( IconType ) );

		foreach ( var type in allTypes )
		{
			var otherTypes = allTypes.Where( x => x != type );
			int otherCount = 0;

			foreach ( var otherType in otherTypes )
			{
				otherCount += Icon.All[otherType].Count;
			}

			if ( otherCount == 0 )
			{
				return type;
			}
		}

		return null;
	}
}
