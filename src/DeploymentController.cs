using SwinGameSDK;


/// <summary>
/// The DeploymentController controls the players actions
/// during the deployment phase.
/// </summary>
sealed class DeploymentController
{
	private const int SHIPS_TOP = 98;
	private const int SHIPS_LEFT = 20;
	private const int SHIPS_HEIGHT = 90;
	private const int SHIPS_WIDTH = 300;
	
	private const int TOP_BUTTONS_TOP = 72;
	private const int TOP_BUTTONS_HEIGHT = 46;
	
	private const int PLAY_BUTTON_LEFT = 693;
	private const int PLAY_BUTTON_WIDTH = 80;
	
	private const int UP_DOWN_BUTTON_LEFT = 410;
	private const int LEFT_RIGHT_BUTTON_LEFT = 350;
	
	private const int RANDOM_BUTTON_LEFT = 547;
	private const int RANDOM_BUTTON_WIDTH = 51;
	
	private const int DIR_BUTTONS_WIDTH = 47;
	
	private const int TEXT_OFFSET = 5;
	
	private static Direction _currentDirection = Direction.UpDown;
	private static ShipName _selectedShip = ShipName.Tug;
	
	/// <summary>
	/// Handles user input for the Deployment phase of the game.
	/// </summary>
	/// <remarks>
	/// Involves selecting the ships, deloying ships, changing the direction
	/// of the ships to add, randomising deployment, end then ending
	/// deployment
	/// </remarks>
	public static void HandleDeploymentInput()
	{
		if (SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
		{
			AddNewState(GameState.ViewingGameMenu);
		}
		
		if (SwinGame.KeyTyped(KeyCode.VK_UP) || SwinGame.KeyTyped(KeyCode.VK_DOWN))
		{
			_currentDirection = Direction.UpDown;
		}
		if (SwinGame.KeyTyped(KeyCode.VK_LEFT) || SwinGame.KeyTyped(KeyCode.VK_RIGHT))
		{
			_currentDirection = Direction.LeftRight;
		}
		
		if (SwinGame.KeyTyped(KeyCode.VK_R))
		{
			HumanPlayer.RandomizeDeployment();
		}
		
		if (SwinGame.MouseClicked(MouseButton.LeftButton))
		{
			ShipName selected = default(ShipName);
			selected = GetShipMouseIsOver();
			if (selected != ShipName.None)
			{
				_selectedShip = selected;
			}
			else
			{
				DoDeployClick();
			}
			
			if (HumanPlayer.ReadyToDeploy && IsMouseInRectangle(PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP, PLAY_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
			{
				EndDeployment();
			}
			else if (IsMouseInRectangle(UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
			{
				_currentDirection = Direction.LeftRight;
			}
			else if (IsMouseInRectangle(LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
			{
				_currentDirection = Direction.LeftRight;
			}
			else if (IsMouseInRectangle(RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP, RANDOM_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
			{
				HumanPlayer.RandomizeDeployment();
			}
		}
	}
	
	/// <summary>
	/// The user has clicked somewhere on the screen, check if its is a deployment and deploy
	/// the current ship if that is the case.
	/// </summary>
	/// <remarks>
	/// If the click is in the grid it deploys to the selected location
	/// with the indicated direction
	/// </remarks>
	private static void DoDeployClick()
	{
		Point2D mouse = default(Point2D);
		
		mouse = SwinGame.MousePosition();
		
		//Calculate the row/col clicked
		int row = 0;
		int col = 0;
		row = System.Convert.ToInt32(Convert.ToInt32(Math.Floor((mouse.Y) / (CELL_HEIGHT + CELL_GAP))));
		col = System.Convert.ToInt32(Convert.ToInt32(Math.Floor(System.Convert.ToDouble(mouse.X - FIELD_LEFT) / System.Convert.ToDouble(CELL_WIDTH + CELL_GAP))));
		
		if (row >= 0 & row < HumanPlayer.PlayerGrid.Height)
		{
			if (col >= 0 & col < HumanPlayer.PlayerGrid.Width)
			{
				//if in the area try to deploy
				try
				{
					HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection);
				}
				catch (Exception ex)
				{
					Audio.PlaySoundEffect(GameSound("Error"));
					Message = ex.Message;
				}
			}
		}
	}
	
	/// <summary>
	/// Draws the deployment screen showing the field and the ships
	/// that the player can deploy.
	/// </summary>
	public static void DrawDeployment()
	{
		DrawField(HumanPlayer.PlayerGrid, HumanPlayer, true);
		
		//Draw the Left/Right and Up/Down buttons
		if (_currentDirection == Direction.LeftRight)
		{
			SwinGame.DrawBitmap(GameImage("LeftRightButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
			//SwinGame.DrawText("U/D", Color.Gray, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
			//SwinGame.DrawText("L/R", Color.White, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
		}
		else
		{
			SwinGame.DrawBitmap(GameImage("UpDownButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
			//SwinGame.DrawText("U/D", Color.White, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
			//SwinGame.DrawText("L/R", Color.Gray, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
		}
		
		//DrawShips
		foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
		{
			int i = 0;
			i = System.Convert.ToInt32(System.Convert.ToInt32(Int(sn)) - 1);
			if (i >= 0)
			{
				if (sn == _selectedShip)
				{
					SwinGame.DrawBitmap(GameImage("SelectedShip"), SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT);
					//    SwinGame.FillRectangle(Color.LightBlue, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
					//Else
					//    SwinGame.FillRectangle(Color.Gray, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
				}
				
				//SwinGame.DrawRectangle(Color.Black, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
				//SwinGame.DrawText(sn.ToString(), Color.Black, GameFont("Courier"), SHIPS_LEFT + TEXT_OFFSET, SHIPS_TOP + i * SHIPS_HEIGHT)
				
			}
		}
		
		if (HumanPlayer.ReadyToDeploy)
		{
			SwinGame.DrawBitmap(GameImage("PlayButton"), PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP);
			//SwinGame.FillRectangle(Color.LightBlue, PLAY_BUTTON_LEFT, PLAY_BUTTON_TOP, PLAY_BUTTON_WIDTH, PLAY_BUTTON_HEIGHT)
			//SwinGame.DrawText("PLAY", Color.Black, GameFont("Courier"), PLAY_BUTTON_LEFT + TEXT_OFFSET, PLAY_BUTTON_TOP)
		}
		
		SwinGame.DrawBitmap(GameImage("RandomButton"), RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP);
		
		DrawMessage();
	}
	
	/// <summary>
	/// Gets the ship that the mouse is currently over in the selection panel.
	/// </summary>
	/// <returns>The ship selected or none</returns>
	private static ShipName GetShipMouseIsOver()
	{
		foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
		{
			int i = 0;
			i = System.Convert.ToInt32(System.Convert.ToInt32(Int(sn)) - 1);
			
			if (IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT))
			{
				return sn;
			}
		}
		
		return ShipName.None;
	}
}