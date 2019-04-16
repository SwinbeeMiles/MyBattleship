using System;
using System.Collections.Generic;
using SwinGameSDK;


/// <summary>
/// The GameController is responsible for controlling the game,
/// managing user input, and displaying the current state of the
/// game.
/// </summary>
public sealed class GameController
{
	
	private static BattleShipsGame _theGame;
	private static Player _human;
	private static AIPlayer _ai;
	
	private static Stack<GameState> _state = new Stack<GameState>();
	
	private static AIOption _aiSetting;
	public static Random rnd = new Random ();
	/// <summary>
	/// Returns the current state of the game, indicating which screen is
	/// currently being used
	/// </summary>
	/// <value>The current state</value>
	/// <returns>The current state</returns>
	public static GameState CurrentState
	{
		get
		{
			return _state.Peek();
		}
	}
	
	/// <summary>
	/// Returns the human player.
	/// </summary>
	/// <value>the human player</value>
	/// <returns>the human player</returns>
	public static Player HumanPlayer
	{
		get
		{
			return _human;
		}
	}
	
	/// <summary>
	/// Returns the computer player.
	/// </summary>
	/// <value>the computer player</value>
	/// <returns>the conputer player</returns>
	public static Player ComputerPlayer
	{
		get
		{
			return _ai;
		}
	}

	/// <summary>
	/// Initializes the state that the player is inhabiting in the main menu.
	/// </summary>
	static GameController()
	{
		//bottom state will be quitting. If player exits main menu then the game is over
		_state.Push(GameState.Quitting);
		
		//at the start the player is viewing the main menu
		_state.Push(GameState.ViewingMainMenu);
	}
	
	/// <summary>
	/// Starts a new game.
	/// </summary>
	/// <remarks>
	/// Creates an AI player based upon the _aiSetting.
	/// </remarks>
	public static void StartGame()
	{
		if (_theGame != null)
		{
			EndGame();
		}
		
		//Create the game
		_theGame = new BattleShipsGame();
		
		//create the players
		if (_aiSetting == AIOption.Medium)
		{
			_ai = new AIMediumPlayer(_theGame);
		}
		else if (_aiSetting == AIOption.Hard)
		{
			_ai = new AIHardPlayer(_theGame);
		}
		else
		{
			_ai = new AIHardPlayer(_theGame);
		}
		
		_human = new Player(_theGame);
		
		//AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed += GridChanged;
		_theGame.AttackCompleted += AttackCompleted;
		
		AddNewState(GameState.Deploying);
	}
	
	/// <summary>
	/// Stops listening to the old game once a new game is started
	/// </summary>
	
	private static void EndGame()
	{
		//RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
		_ai.PlayerGrid.Changed -= GridChanged;
		_theGame.AttackCompleted -= AttackCompleted;
	}
	
	/// <summary>
	/// Listens to the game grids for any changes and redraws the screen
	/// when the grids change
	/// </summary>
	/// <param name="sender">the grid that changed</param>
	/// <param name="args">not used</param>
	private static void GridChanged(object sender, EventArgs args)
	{
		DrawScreen();
		SwinGame.RefreshScreen();
	}

	/// <summary>
	/// Plays the corresponding animation sequence and sound if an explosion animation was carried out.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="column">Column.</param>
	/// <param name="showAnimation">If set to <c>true</c> show animation.</param>
	private static void PlayHitSequence(int row, int column, bool showAnimation)
	{

		if (showAnimation)
		{
			UtilityFunctions.AddExplosion(row, column);
		}
		
		Audio.PlaySoundEffect(GameResources.GameSound("Hit"));
		UtilityFunctions.DrawAnimationSequence();
	}

	/// <summary>
	/// Plays the corresponding animation sequence and sound if a splash animation was carried out.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="column">Column.</param>
	/// <param name="showAnimation">If set to <c>true</c> show animation.</param>
	private static void PlayMissSequence(int row, int column, bool showAnimation)
	{
		if (showAnimation)
		{
			UtilityFunctions.AddSplash(row, column);
		}
		
		Audio.PlaySoundEffect(GameResources.GameSound("Miss"));
		
		UtilityFunctions.DrawAnimationSequence();
	}
	
	/// <summary>
	/// Listens for attacks to be completed.
	/// </summary>
	/// <param name="sender">the game</param>
	/// <param name="result">the result of the attack</param>
	/// <remarks>
	/// Displays a message, plays sound and redraws the screen
	/// </remarks>
	private static void AttackCompleted(object sender, AttackResult result)
	{
		int randomLose = rnd.Next (1, 3);
		int randomDestroy = rnd.Next (1, 4);
		bool isHuman = false;
		isHuman = ReferenceEquals(_theGame.Player, HumanPlayer);
		
		if (isHuman)
		{
			UtilityFunctions.Message = "You " + result.ToString();
		}
		else
		{
			UtilityFunctions.Message = "The AI " + result.ToString();
		}
		
		if (result.Value == ResultOfAttack.Destroyed)
		{
			PlayHitSequence(System.Convert.ToInt32(result.Row), System.Convert.ToInt32(result.Column), isHuman);
			Audio.PlaySoundEffect(GameResources.GameSound("Sink"));

			if (randomDestroy == 1) 
			{
				Audio.PlaySoundEffect (GameResources.GameSound ("Destroy1"));
			} 

			else if (randomDestroy == 2) 
			{
				Audio.PlaySoundEffect (GameResources.GameSound ("Destroy2"));
			} 

			else if (randomDestroy == 3) 
			{
				Audio.PlaySoundEffect (GameResources.GameSound ("Destroy3"));
			} 

			else 
			{
				Audio.PlaySoundEffect (GameResources.GameSound ("Destroy4"));
			}

		}
		else if (result.Value == ResultOfAttack.GameOver)
		{
			PlayHitSequence(System.Convert.ToInt32(result.Row), System.Convert.ToInt32(result.Column), isHuman);
			Audio.PlaySoundEffect(GameResources.GameSound("Sink"));
			
			while (Audio.SoundEffectPlaying(GameResources.GameSound("Sink")))
			{
				SwinGame.Delay(10);
				SwinGame.RefreshScreen();
			}
			
			if (HumanPlayer.IsDestroyed)
			{
				Audio.PlaySoundEffect(GameResources.GameSound("Lose"));
				if (randomLose == 1) {
					Audio.PlaySoundEffect (GameResources.GameSound ("Lose1"));
				} else {
					Audio.PlaySoundEffect (GameResources.GameSound ("Lose2"));
				}

			}
			else
			{
				Audio.PlaySoundEffect(GameResources.GameSound("Winner"));
				Audio.PlaySoundEffect (GameResources.GameSound ("Win1"));
			}
		}
		else if (result.Value == ResultOfAttack.Hit)
		{
			PlayHitSequence(System.Convert.ToInt32(result.Row), System.Convert.ToInt32(result.Column), isHuman);
		}
		else if (result.Value == ResultOfAttack.Miss)
		{
			PlayMissSequence(System.Convert.ToInt32(result.Row), System.Convert.ToInt32(result.Column), isHuman);
		}
		else if (result.Value == ResultOfAttack.ShotAlready)
		{
			Audio.PlaySoundEffect(GameResources.GameSound("Error"));
		}
	}
	
	/// <summary>
	/// Completes the deployment phase of the game and
	/// switches to the battle mode (Discovering state)
	/// </summary>
	/// <remarks>
	/// This adds the players to the game before switching
	/// state.
	/// </remarks>
	public static void EndDeployment()
	{
		int randStart = rnd.Next (1, 4);
		//deploy the players
		_theGame.AddDeployedPlayer(_human);
		_theGame.AddDeployedPlayer(_ai);

		SwitchState (GameState.Discovering);
		Audio.PlaySoundEffect (GameResources.GameSound ("Siren"));
		if (randStart == 1) 
		{
			Audio.PlaySoundEffect (GameResources.GameSound ("Start1"));
		} 

		else if (randStart == 2)
		{
			Audio.PlaySoundEffect (GameResources.GameSound ("Start2"));
		} 

		else
		{
			Audio.PlaySoundEffect (GameResources.GameSound ("Start3"));
		}

	}

	/// <summary>
	/// Gets the player to attack the indicated row and column.
	/// </summary>
	/// <param name="row">the row to attack</param>
	/// <param name="col">the column to attack</param>
	/// <remarks>
	/// Checks the attack result once the attack is complete
	/// </remarks>
	public static void Attack(int row, int col)
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Shoot(row, col);
		CheckAttackResult(result);
	}
	
	/// <summary>
	/// Gets the AI to attack.
	/// </summary>
	/// <remarks>
	/// Checks the attack result once the attack is complete.
	/// </remarks>
	private static void AIAttack()
	{
		AttackResult result = default(AttackResult);
		result = _theGame.Player.Attack();
		CheckAttackResult(result);
	}
	
	/// <summary>
	/// Checks the results of the attack and switches to
	/// Ending the Game if the result was game over.
	/// </summary>
	/// <param name="result">the result of the last
	/// attack</param>
	/// <remarks>Gets the AI to attack if the result switched
	/// to the AI player.</remarks>
	private static void CheckAttackResult(AttackResult result)
	{
		if (result.Value == ResultOfAttack.Miss) 
		{
			if (ReferenceEquals (_theGame.Player, ComputerPlayer)) 
			{
				AIAttack ();
			}
		} 

		else if (result.Value == ResultOfAttack.GameOver)
		{
			SwitchState(GameState.EndingGame);
		}
	}
	
	/// <summary>
	/// Handles the user SwinGame.
	/// </summary>
	/// <remarks>
	/// Reads key and mouse input and converts these into
	/// actions for the game to perform. The actions
	/// performed depend upon the state of the game.
	/// </remarks>
	public static void HandleUserInput()
	{
		//Read incoming input events
		SwinGame.ProcessEvents();
		
		if (CurrentState == GameState.ViewingMainMenu)
		{
			MenuController.HandleMainMenuInput();
		}
		else if (CurrentState == GameState.ViewingGameMenu)
		{
			MenuController.HandleGameMenuInput();
		}
		else if (CurrentState == GameState.AlteringSettings)
		{
			MenuController.HandleSetupMenuInput();
		}
		else if (CurrentState == GameState.Deploying)
		{
			DeploymentController.HandleDeploymentInput();
		}
		else if (CurrentState == GameState.Discovering)
		{
			DiscoveryController.HandleDiscoveryInput();
		}
		else if (CurrentState == GameState.EndingGame)
		{
			EndingGameController.HandleEndOfGameInput();
		}
		else if (CurrentState == GameState.ViewingHighScores)
		{
			HighScoreController.HandleHighScoreInput();
		}
		
		UtilityFunctions.UpdateAnimations();
	}
	
	/// <summary>
	/// Draws the current state of the game to the screen.
	/// </summary>
	/// <remarks>
	/// What is drawn depends upon the state of the game.
	/// </remarks>
	public static void DrawScreen()
	{
		UtilityFunctions.DrawBackground();
		
		if (CurrentState == GameState.ViewingMainMenu)
		{
			MenuController.DrawMainMenu();
		}
		else if (CurrentState == GameState.ViewingGameMenu)
		{
			MenuController.DrawGameMenu();
		}
		else if (CurrentState == GameState.AlteringSettings)
		{
			MenuController.DrawSettings();
		}
		else if (CurrentState == GameState.Deploying)
		{
			DeploymentController.DrawDeployment();
		}
		else if (CurrentState == GameState.Discovering)
		{
			DiscoveryController.DrawDiscovery();
		}
		else if (CurrentState == GameState.EndingGame)
		{
			EndingGameController.DrawEndOfGame();
		}
		else if (CurrentState == GameState.ViewingHighScores)
		{
			HighScoreController.DrawHighScores();
		}
		
		UtilityFunctions.DrawAnimations();
		
		SwinGame.RefreshScreen(120);
	}
	
	/// <summary>
	/// Move the game to a new state. The current state is maintained
	/// so that it can be returned to.
	/// </summary>
	/// <param name="state">the new game state</param>
	public static void AddNewState(GameState state)
	{
		_state.Push(state);
		UtilityFunctions.Message = "";
	}
	
	/// <summary>
	/// End the current state and add in the new state.
	/// </summary>
	/// <param name="newState">the new state of the game</param>
	public static void SwitchState(GameState newState)
	{
		EndCurrentState();
		AddNewState(newState);
	}
	
	/// <summary>
	/// Ends the current state, returning to the prior state
	/// </summary>
	public static void EndCurrentState()
	{
		_state.Pop();
	}
	
	/// <summary>
	/// Sets the difficulty for the next level of the game.
	/// </summary>
	/// <param name="setting">the new difficulty level</param>
	public static void SetDifficulty(AIOption setting)
	{
		_aiSetting = setting;
	}

}