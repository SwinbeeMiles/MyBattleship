using System;
using SwinGameSDK;


/// <summary>
/// The battle phase is handled by the DiscoveryController.
/// </summary>
static class DiscoveryController
{

	/// <summary>
	/// Handles input during the discovery phase of the game.
	/// </summary>
	/// <remarks>
	/// Escape opens the game menu. Clicking the mouse will
	/// attack a location.
	/// </remarks>
	public static void HandleDiscoveryInput ()
	{
		if (SwinGame.KeyTyped (KeyCode.vk_ESCAPE)) {
			GameController.AddNewState (GameState.ViewingGameMenu);
		}

		if (SwinGame.MouseClicked (MouseButton.LeftButton)) {
			DoAttack ();
		}
	}

	/// <summary>
	/// Attack the location that the mouse if over.
	/// </summary>
	private static void DoAttack ()
	{
		Point2D mouse = default (Point2D);

		mouse = SwinGame.MousePosition ();

		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = System.Convert.ToInt32 (System.Convert.ToInt32 (System.Math.Floor (System.Convert.ToDouble (mouse.Y - UtilityFunctions.FIELD_TOP) / System.Convert.ToDouble (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP))));
		col = System.Convert.ToInt32 (System.Convert.ToInt32 (System.Math.Floor (System.Convert.ToDouble (mouse.X - UtilityFunctions.FIELD_LEFT) / System.Convert.ToDouble (UtilityFunctions.CELL_WIDTH + UtilityFunctions.CELL_GAP))));

		if (row >= 0 & row < GameController.HumanPlayer.EnemyGrid.Height) {
			if (col >= 0 & col < GameController.HumanPlayer.EnemyGrid.Width) {
				GameController.Attack (row, col);
			}
		}
	}

	/// <summary>
	/// Draws the game during the attack phase.
	/// </summary>s
	public static void DrawDiscovery ()
	{
		const int SCORES_LEFT = 172;
		const int SHOTS_TOP = 157;
		const int HITS_TOP = 206;
		const int SPLASH_TOP = 256;

		if ((SwinGame.KeyDown (KeyCode.vk_LSHIFT) || SwinGame.KeyDown (KeyCode.vk_RSHIFT)) && SwinGame.KeyDown (KeyCode.vk_C)) {
			UtilityFunctions.DrawField (GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, true);
		} else {
			UtilityFunctions.DrawField (GameController.HumanPlayer.EnemyGrid, GameController.ComputerPlayer, false);
		}

		UtilityFunctions.DrawSmallField (GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer);
		UtilityFunctions.DrawMessage ();

		SwinGame.DrawText (GameController.HumanPlayer.Shots.ToString (), Color.White, GameResources.GameFont ("Menu"), SCORES_LEFT, SHOTS_TOP);
		SwinGame.DrawText (GameController.HumanPlayer.Hits.ToString (), Color.White, GameResources.GameFont ("Menu"), SCORES_LEFT, HITS_TOP);
		SwinGame.DrawText (GameController.HumanPlayer.Missed.ToString (), Color.White, GameResources.GameFont ("Menu"), SCORES_LEFT, SPLASH_TOP);
	}

}