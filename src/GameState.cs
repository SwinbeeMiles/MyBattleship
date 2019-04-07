/// <summary>
/// The GameStates represent the state of the Battleships game play.
/// This is used to control the actions and view displayed to
/// the player.
/// </summary>

public enum GameState
{
	/// <summary>
	/// The player is viewing the main menu.
	/// </summary>
	ViewingMainMenu,
	
	/// <summary>
	/// The player is viewing the game menu
	/// </summary>
	ViewingGameMenu,
	
	/// <summary>
	/// The player is looking at the high scores
	/// </summary>
	ViewingHighScores,
	
	/// <summary>
	/// The player is altering the game settings
	/// </summary>
	AlteringSettings,
	
	/// <summary>
	/// Players are deploying their ships
	/// </summary>
	Deploying,
	
	/// <summary>
	/// Players are attempting to locate each others ships
	/// </summary>
	Discovering,
	
	/// <summary>
	/// One player has won, showing the victory screen
	/// </summary>
	EndingGame,
	
	/// <summary>
	/// The player has quit. Show ending credits and terminate the game
	/// </summary>
	Quitting
}