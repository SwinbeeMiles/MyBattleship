using SwinGameSDK;


/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

sealed class EndingGameController
{
	
	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public static void DrawEndOfGame()
	{
		DrawField(ComputerPlayer.PlayerGrid, ComputerPlayer, true);
		DrawSmallField(HumanPlayer.PlayerGrid, HumanPlayer);
		
		if (HumanPlayer.IsDestroyed)
		{
			SwinGame.DrawTextLines("YOU LOSE!", Color.White, Color.Transparent, GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
		else
		{
			SwinGame.DrawTextLines("-- WINNER --", Color.White, Color.Transparent, GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight());
		}
	}
	
	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highsSwinGame.
	/// </summary>
	public static void HandleEndOfGameInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) 
			|| SwinGame.KeyTyped(KeyCode.VK_RETURN) 
			|| SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
		{
			ReadHighScore(HumanPlayer.Score);
			EndCurrentState();
		}
	}
	
}