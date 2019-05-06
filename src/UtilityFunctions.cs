using SwinGameSDK;
using System.Collections.Generic;

/// <summary>
/// This includes a number of utility methods for
/// drawing and interacting with the Mouse.
/// </summary>
static class UtilityFunctions
{
	public const int FIELD_TOP = 122;
	public const int FIELD_LEFT = 349;
	public const int FIELD_WIDTH = 418;
	public const int FIELD_HEIGHT = 418;

	public const int MESSAGE_TOP = 548;

	public const int CELL_WIDTH = 40;
	public const int CELL_HEIGHT = 40;
	public const int CELL_GAP = 2;

	public const int SHIP_GAP = 3;

	private readonly static Color SMALL_SEA = SwinGame.RGBAColor (6, 60, 94, 255);
	private readonly static Color SMALL_SHIP = Color.Gray;
	private readonly static Color SMALL_MISS = SwinGame.RGBAColor (1, 147, 220, 255);
	private readonly static Color SMALL_HIT = SwinGame.RGBAColor (169, 24, 37, 255);

	private readonly static Color LARGE_SEA = SwinGame.RGBAColor (6, 60, 94, 255);
	private readonly static Color LARGE_SHIP = Color.Gray;
	private readonly static Color LARGE_MISS = SwinGame.RGBAColor (1, 147, 220, 255);
	private readonly static Color LARGE_HIT = SwinGame.RGBAColor (252, 2, 3, 255);

	private readonly static Color OUTLINE_COLOR = SwinGame.RGBAColor (5, 55, 88, 255);
	private readonly static Color SHIP_FILL_COLOR = Color.Gray;
	private readonly static Color SHIP_OUTLINE_COLOR = Color.White;
	private readonly static Color MESSAGE_COLOR = SwinGame.RGBAColor (2, 167, 252, 255);

	public const int ANIMATION_CELLS = 6;
	public const int FRAMES_PER_CELL = 10;

	/// <summary>
	/// Determines if the mouse is in a given rectangle.
	/// </summary>
	/// <param name="x">the x location to check</param>
	/// <param name="y">the y location to check</param>
	/// <param name="w">the width to check</param>
	/// <param name="h">the height to check</param>
	/// <returns>true if the mouse is in the area checked</returns>
	public static bool IsMouseInRectangle (int x, int y, int w, int h)
	{
		Point2D mouse = default (Point2D);
		bool result = false;

		mouse = SwinGame.MousePosition ();

		//if the mouse is inline with the button horizontally
		if (mouse.X >= x && mouse.X <= x + w) {
			//Check vertical position
			if (mouse.Y >= y && mouse.Y <= y + h) {
				result = true;
			}
		}

		return result;
	}

	/// <summary>
	/// Draws a large field using the grid and the indicated player's ships.
	/// </summary>
	/// <param name="grid">the grid to draw</param>
	/// <param name="thePlayer">the players ships to show</param>
	/// <param name="showShips">indicates if the ships should be shown</param>
	public static void DrawField (ISeaGrid grid, Player thePlayer, bool showShips)
	{
		DrawCustomField (grid, thePlayer, false, showShips, FIELD_LEFT, FIELD_TOP, FIELD_WIDTH, FIELD_HEIGHT, CELL_WIDTH, CELL_HEIGHT, CELL_GAP);
	}

	/// <summary>
	/// Draws a small field, showing the attacks made and the locations of the player's ships
	/// </summary>
	/// <param name="grid">the grid to show</param>
	/// <param name="thePlayer">the player to show the ships of</param>
	public static void DrawSmallField (ISeaGrid grid, Player thePlayer)
	{
		const int SMALL_FIELD_LEFT = 39;
		const int SMALL_FIELD_TOP = 373;
		const int SMALL_FIELD_WIDTH = 166;
		const int SMALL_FIELD_HEIGHT = 166;
		const int SMALL_FIELD_CELL_WIDTH = 13;
		const int SMALL_FIELD_CELL_HEIGHT = 13;
		const int SMALL_FIELD_CELL_GAP = 4;

		DrawCustomField (grid, thePlayer, true, true, SMALL_FIELD_LEFT, SMALL_FIELD_TOP, SMALL_FIELD_WIDTH, SMALL_FIELD_HEIGHT, SMALL_FIELD_CELL_WIDTH, SMALL_FIELD_CELL_HEIGHT, SMALL_FIELD_CELL_GAP);
	}

	/// <summary>
	/// Draws the player's grid and ships.
	/// </summary>
	/// <param name="grid">the grid to show</param>
	/// <param name="thePlayer">the player to show the ships of</param>
	/// <param name="small">true if the small grid is shown</param>
	/// <param name="showShips">true if ships are to be shown</param>
	/// <param name="left">the left side of the grid</param>
	/// <param name="top">the top of the grid</param>
	/// <param name="width">the width of the grid</param>
	/// <param name="height">the height of the grid</param>
	/// <param name="cellWidth">the width of each cell</param>
	/// <param name="cellHeight">the height of each cell</param>
	/// <param name="cellGap">the gap between the cells</param>
	private static void DrawCustomField (ISeaGrid grid, Player thePlayer, bool small, bool showShips, int left, int top, int width, int height, int cellWidth, int cellHeight, int cellGap)
	{
		//SwinGame.FillRectangle(Color.Blue, left, top, width, height)

		int rowTop = 0;
		int colLeft = 0;

		//Draw the grid
		for (int row = 0; row <= 9; row++) {
			rowTop = top + (cellGap + cellHeight) * row;

			for (int col = 0; col <= 9; col++) {
				colLeft = left + (cellGap + cellWidth) * col;

				Color fillColor = default (Color);
				bool draw = false;

				draw = true;

				if (grid.Item (row, col) == TileView.Ship) {
					draw = false;
					//If small Then fillColor = _SMALL_SHIP Else fillColor = _LARGE_SHIP
				} else if (grid.Item (row, col) == TileView.Miss) {
					if (small) {
						fillColor = SMALL_MISS;
					} else {
						fillColor = LARGE_MISS;
					}
				} else if (grid.Item (row, col) == TileView.Hit) {
					if (small) {
						fillColor = SMALL_HIT;
					} else {
						fillColor = LARGE_HIT;
					}
				} else if ((grid.Item (row, col) == TileView.Sea) || (grid.Item (row, col) == TileView.Ship)) {
					if (small) {
						fillColor = SMALL_SEA;
					} else {
						draw = false;
					}
				}

				if (draw) {
					SwinGame.FillRectangle (fillColor, colLeft, rowTop, cellWidth, cellHeight);
					if (!small) {
						SwinGame.DrawRectangle (OUTLINE_COLOR, colLeft, rowTop, cellWidth, cellHeight);
					}
				}
			}
		}

		if (!showShips) {
			return;
		}

		int shipHeight = 0;
		int shipWidth = 0;
		string shipName = "";

		//Draw the ships
		foreach (Ship s in thePlayer) {
			if (ReferenceEquals (s, null) || !s.IsDeployed) {
				continue;
			}
			rowTop = top + (cellGap + cellHeight) * s.Row + SHIP_GAP;
			colLeft = left + (cellGap + cellWidth) * s.Column + SHIP_GAP;

			if (s.Direction == Direction.LeftRight) {
				shipName = "ShipLR" + s.Size;
				shipHeight = cellHeight - (SHIP_GAP * 2);
				shipWidth = System.Convert.ToInt32 ((cellWidth + cellGap) * s.Size - (SHIP_GAP * 2) - cellGap);
			} else {
				//Up down
				shipName = "ShipUD" + s.Size;
				shipHeight = System.Convert.ToInt32 ((cellHeight + cellGap) * s.Size - (SHIP_GAP * 2) - cellGap);
				shipWidth = cellWidth - (SHIP_GAP * 2);
			}

			if (!small) {
				SwinGame.DrawBitmap (GameResources.GameImage (shipName), colLeft, rowTop);
			} else {
				SwinGame.FillRectangle (SHIP_FILL_COLOR, colLeft, rowTop, shipWidth, shipHeight);
				SwinGame.DrawRectangle (SHIP_OUTLINE_COLOR, colLeft, rowTop, shipWidth, shipHeight);
			}
		}
	}

	private static string _message;

	/// <summary>
	/// The message to display
	/// </summary>
	/// <value>The message to display</value>
	/// <returns>The message to display</returns>
	public static string Message {
		get {
			return _message;
		}
		set {
			_message = value;
		}
	}

	/// <summary>
	/// Draws the message to the screen
	/// </summary>
	public static void DrawMessage ()
	{
		SwinGame.DrawText (Message, MESSAGE_COLOR, GameResources.GameFont ("Courier"), FIELD_LEFT, MESSAGE_TOP);
	}

	/// <summary>
	/// Draws the background for the current state of the game
	/// </summary>
	public static void DrawBackground ()
	{

		if ((((GameController.CurrentState == GameState.ViewingMainMenu) || (GameController.CurrentState == GameState.ViewingGameMenu)) || (GameController.CurrentState == GameState.AlteringSettings)) || (GameController.CurrentState == GameState.ViewingHighScores)) {
			SwinGame.DrawBitmap (GameResources.GameImage ("Menu"), 0, 0);
		} else if ((GameController.CurrentState == GameState.Discovering) || (GameController.CurrentState == GameState.EndingGame)) {
			SwinGame.DrawBitmap (GameResources.GameImage ("Discovery"), 0, 0);
		} else if (GameController.CurrentState == GameState.Deploying) {
			SwinGame.DrawBitmap (GameResources.GameImage ("Deploy"), 0, 0);
		} else if (GameController.CurrentState == GameState.ViewingInstructions) {
			SwinGame.DrawBitmap (GameResources.GameImage ("Instructions"), 0, 0);
		} else {
			SwinGame.ClearScreen ();
		}

		SwinGame.DrawFramerate (585, 580, GameResources.GameFont ("CourierSmall"));
	}

	/// <summary>
	/// Adds the explosion animation at the given position.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	public static void AddExplosion (int row, int col)
	{
		AddAnimation (row, col, "Explosion");
	}

	/// <summary>
	/// Adds the splash animation at the given position.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	public static void AddSplash (int row, int col)
	{
		AddAnimation (row, col, "Splash");
	}

	private static List<Sprite> _Animations = new List<Sprite> ();

	/// <summary>
	/// Adds a specificed animation to a given position.
	/// </summary>
	/// <param name="row">Row.</param>
	/// <param name="col">Col.</param>
	/// <param name="image">Image.</param>
	private static void AddAnimation (int row, int col, string image)
	{
		Sprite s = default (Sprite);
		Bitmap imgObj = default (Bitmap);

		imgObj = GameResources.GameImage (image);
		imgObj.SetCellDetails (40, 40, 3, 3, 7);

		AnimationScript animation = default (AnimationScript);
		animation = SwinGame.LoadAnimationScript ("splash.txt");

		s = SwinGame.CreateSprite (imgObj, animation);
		s.X = FIELD_LEFT + col * (CELL_WIDTH + CELL_GAP);
		s.Y = FIELD_TOP + row * (CELL_HEIGHT + CELL_GAP);

		s.StartAnimation ("splash");
		_Animations.Add (s);
	}

	/// <summary>
	/// Updates the animation that has been specified on a select tile.
	/// </summary>
	public static void UpdateAnimations ()
	{
		List<Sprite> ended = new List<Sprite> ();
		foreach (Sprite s in _Animations) {
			SwinGame.UpdateSprite (s);
			if (s.AnimationHasEnded) {
				ended.Add (s);
			}
		}

		foreach (Sprite s in ended) {
			_Animations.Remove (s);
			SwinGame.FreeSprite (s);
		}
	}

	/// <summary>
	/// Draws the animation that that had been specificed on a select tile.
	/// </summary>
	public static void DrawAnimations ()
	{
		foreach (Sprite s in _Animations) {
			SwinGame.DrawSprite (s);
		}
	}

	/// <summary>
	/// Draws the the animation sequence that has been specified on a select tile.
	/// </summary>
	public static void DrawAnimationSequence ()
	{
		int i = 0;
		for (i = 1; i <= ANIMATION_CELLS * FRAMES_PER_CELL; i++) {
			UpdateAnimations ();
			GameController.DrawScreen ();
		}
	}
}