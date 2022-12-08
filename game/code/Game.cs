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
	public float Zoom = 2f;

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
