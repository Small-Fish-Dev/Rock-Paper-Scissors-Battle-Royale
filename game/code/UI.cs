﻿using Sandbox.Internal;

namespace RockPaperScissors;

public partial class UI
{
	[Event( "StateChange" )]
	void ChangeState( GameState newState )
	{
		if ( newState == GameState.Playing )
		{
			for ( int i = 0; i < ( RockPaperScissors.Game.Zoom / 0.01f ); i++ )
			{
				var allTypes = Enum.GetValues( typeof( IconType ) );
				var selectedType = (IconType)allTypes.GetValue( Rand.Int( allTypes.Length - 1 ) );
				var icon = new Icon( selectedType );
				AddChild( icon );
			}
		}
	}
}
