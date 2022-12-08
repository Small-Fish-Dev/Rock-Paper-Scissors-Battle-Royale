using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RockPaperScissors;

public enum GameState
{
	Paused,
	Playing,
	Ending
}

public partial class RockPaperScissors : GameManager
{
	public static RockPaperScissors Game;
	[Net, Change] internal GameState state { get; set; } = GameState.Paused;
	public GameState State
	{
		get => state;
		set
		{
			state = value;
			Event.Run( "StateChange", value );
		}
	}

	public RockPaperScissors()
	{
		Game = this;
		if ( IsClient )
		{
			new UI();
		}
	}

	public override void ClientJoined( Client client )
	{
		State = GameState.Playing;
	}

	public void OnstateChanged( GameState oldState, GameState newState )
	{ 
		Event.Run( "StateChange", newState );
	}
}
