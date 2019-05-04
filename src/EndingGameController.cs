using System;
using System.Collections.Generic;
using SwinGameSDK;

/// <summary>
/// The EndingGameController is responsible for managing the interactions at the end
/// of a game.
/// </summary>

static class EndingGameController
{

	/// <summary>
	/// Draw the end of the game screen, shows the win/lose state
	/// </summary>
	public static void DrawEndOfGame ()
	{
		UtilityFunctions.DrawField (GameController.ComputerPlayer.PlayerGrid, GameController.ComputerPlayer, true);
		UtilityFunctions.DrawSmallField (GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);

		if ((int)SwinGame.TimerTicks(GameController.GameTimer) >= 180000) {
			SwinGame.DrawBitmap (GameResources.GameImage ("YouLose"), 175, 175);
		} 
		else if (GameController.HumanPlayer.IsDestroyed) {
			SwinGame.DrawBitmap (GameResources.GameImage ("YouLose"), 175, 175);
		} 
		else {
			SwinGame.DrawBitmap (GameResources.GameImage ("YouWin"), 175, 175);
		}
	}

	/// <summary>
	/// Handle the input during the end of the game. Any interaction
	/// will result in it reading in the highsSwinGame.
	/// </summary>
	public static void HandleEndOfGameInput ()
	{
		if (SwinGame.MouseClicked (MouseButton.LeftButton)
			|| SwinGame.KeyTyped (KeyCode.vk_RETURN)
			|| SwinGame.KeyTyped (KeyCode.vk_ESCAPE)) {
			HighScoreController.ReadHighScore (GameController.HumanPlayer.Score);
			GameController.EndCurrentState ();
		}
	}

}