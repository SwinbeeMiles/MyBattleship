using System.Collections.Generic;
using SwinGameSDK;

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

    private static readonly Color SMALL_SEA = SwinGame.RGBAColor(6, 60, 94, 255);
    private static readonly Color SMALL_SHIP = Color.Gray;
    private static readonly Color SMALL_MISS = SwinGame.RGBAColor(1, 147, 220, 255);
    private static readonly Color SMALL_HIT = SwinGame.RGBAColor(169, 24, 37, 255);

    private static readonly Color LARGE_SEA = SwinGame.RGBAColor(6, 60, 94, 255);
    private static readonly Color LARGE_SHIP = Color.Gray;
    private static readonly Color LARGE_MISS = SwinGame.RGBAColor(1, 147, 220, 255);
    private static readonly Color LARGE_HIT = SwinGame.RGBAColor(252, 2, 3, 255);

    private static readonly Color OUTLINE_COLOR = SwinGame.RGBAColor(5, 55, 88, 255);
    private static readonly Color SHIP_FILL_COLOR = Color.Gray;
    private static readonly Color SHIP_OUTLINE_COLOR = Color.White;
    private static readonly Color MESSAGE_COLOR = SwinGame.RGBAColor(2, 167, 252, 255);

    public const int ANIMATION_CELLS = 7;
    public const int FRAMES_PER_CELL = 8;

    /// <summary>
    /// Determines if the mouse is in a given rectangle.
    /// </summary>
    /// <param name="x">the x location to check</param>
    /// <param name="y">the y location to check</param>
    /// <param name="w">the width to check</param>
    /// <param name="h">the height to check</param>
    /// <returns>true if the mouse is in the area checked</returns>
    public static bool IsMouseInRectangle(int x, int y, int w, int h)
    {
        Point2D mouse;
        bool result = false;

        mouse = SwinGame.MousePosition();

        // if the mouse is inline with the button horizontally
        if (mouse.X >= x & mouse.X <= x + w)
        {
            // Check vertical position
            if (mouse.Y >= y & mouse.Y <= y + h)
                result = true;
        }

        return result;
    }

    /// <summary>
    /// Draws a large field using the grid and the indicated player's ships.
    /// </summary>
    /// <param name="grid">the grid to draw</param>
    /// <param name="thePlayer">the players ships to show</param>
    /// <param name="showShips">indicates if the ships should be shown</param>
    public static void DrawField(ISeaGrid grid, Player thePlayer, bool showShips)
    {
        DrawCustomField(grid, thePlayer, false, showShips, FIELD_LEFT, FIELD_TOP, FIELD_WIDTH, FIELD_HEIGHT, CELL_WIDTH, CELL_HEIGHT, CELL_GAP);
    }

    /// <summary>
    /// Draws a small field, showing the attacks made and the locations of the player's ships
    /// </summary>
    /// <param name="grid">the grid to show</param>
    /// <param name="thePlayer">the player to show the ships of</param>
    public static void DrawSmallField(ISeaGrid grid, Player thePlayer)
    {
        const int SMALL_FIELD_LEFT = 39;
        const int SMALL_FIELD_TOP = 373;
        const int SMALL_FIELD_WIDTH = 166;
        const int SMALL_FIELD_HEIGHT = 166;
        const int SMALL_FIELD_CELL_WIDTH = 13;
        const int SMALL_FIELD_CELL_HEIGHT = 13;
        const int SMALL_FIELD_CELL_GAP = 4;

        DrawCustomField(grid, thePlayer, true, true, SMALL_FIELD_LEFT, SMALL_FIELD_TOP, SMALL_FIELD_WIDTH, SMALL_FIELD_HEIGHT, SMALL_FIELD_CELL_WIDTH, SMALL_FIELD_CELL_HEIGHT, SMALL_FIELD_CELL_GAP);
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
    private static void DrawCustomField(ISeaGrid grid, Player thePlayer, bool small, bool showShips, int left, int top, int width, int height, int cellWidth, int cellHeight, int cellGap)
    {
        // SwinGame.FillRectangle(Color.Blue, left, top, width, height)

        int rowTop;
        int colLeft;

        // Draw the grid
        for (int row = 0; row <= 9; row++)
        {
            rowTop = top + (cellGap + cellHeight) * row;

            for (int col = 0; col <= 9; col++)
            {
                colLeft = left + (cellGap + cellWidth) * col;

                Color fillColor = null;
                bool draw;

                draw = true;

                switch (grid.Item(row, col))
                {
                    case TileView.Ship:
                    {
                        draw = false;
                        break;
                    }

                    case TileView.Miss:
                    {
                        if (small)
                            fillColor = SMALL_MISS;
                        else
                            fillColor = LARGE_MISS;
                        break;
                    }

                    case TileView.Hit:
                    {
                        if (small)
                            fillColor = SMALL_HIT;
                        else
                            fillColor = LARGE_HIT;
                        break;
                    }

                    case TileView.Sea:
                    {
						if (grid.Item (row, col) == TileView.Ship) {
							if (small)
								fillColor = SMALL_SEA;
							else
								draw = false;
							break;
						} 

						break;
                    }
                }

				if (draw)
                {
                    SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight);
                    if (!small)
                        SwinGame.DrawRectangle(OUTLINE_COLOR, colLeft, rowTop, cellWidth, cellHeight);
                }
            }
        }

        if (!showShips)
            return;

        int shipHeight, shipWidth;
        string shipName;

        // Draw the ships
        foreach (Ship s in thePlayer)
        {
            if (s == null || !s.IsDeployed)
                continue;
            rowTop = top + (cellGap + cellHeight) * s.Row + SHIP_GAP;
            colLeft = left + (cellGap + cellWidth) * s.Column + SHIP_GAP;

            if (s.Direction == Direction.LeftRight)
            {
                shipName = "ShipLR" + s.Size;
                shipHeight = cellHeight - (SHIP_GAP * 2);
                shipWidth = (cellWidth + cellGap) * s.Size - (SHIP_GAP * 2) - cellGap;
            }
            else
            {
                // Up down
                shipName = "ShipUD" + s.Size;
                shipHeight = (cellHeight + cellGap) * s.Size - (SHIP_GAP * 2) - cellGap;
                shipWidth = cellWidth - (SHIP_GAP * 2);
            }

            if (!small)
                SwinGame.DrawBitmap(GameResources.GameImage(shipName), colLeft, rowTop);
            else
            {
                SwinGame.FillRectangle(SHIP_FILL_COLOR, colLeft, rowTop, shipWidth, shipHeight);
                SwinGame.DrawRectangle(SHIP_OUTLINE_COLOR, colLeft, rowTop, shipWidth, shipHeight);
            }
        }
    }

    private static string _message;

    /// <summary>
    ///     ''' The message to display
    ///     ''' </summary>
    ///     ''' <value>The message to display</value>
    ///     ''' <returns>The message to display</returns>
    public static string Message
    {
        get
        {
            return _message;
        }
        set
        {
            _message = value;
        }
    }

    /// <summary>
    ///     ''' Draws the message to the screen
    ///     ''' </summary>
    public static void DrawMessage()
    {
        SwinGame.DrawText(Message, MESSAGE_COLOR, GameResources.GameFont("Courier"), FIELD_LEFT, MESSAGE_TOP);
    }

    /// <summary>
    ///     ''' Draws the background for the current state of the game
    ///     ''' </summary>
    public static void DrawBackground()
    {
        switch (GameController.CurrentState)
        {
            case GameState.ViewingMainMenu:
            case GameState.ViewingGameMenu:
            case GameState.AlteringSettings:
            case GameState.ViewingHighScores:
                {
                    SwinGame.DrawBitmap(GameResources.GameImage("Menu"), 0, 0);
                    break;
                }

            case GameState.Discovering:
            case GameState.EndingGame:
                {
                    SwinGame.DrawBitmap(GameResources.GameImage("Discovery"), 0, 0);
                    break;
                }

            case GameState.Deploying:
                {
                    SwinGame.DrawBitmap(GameResources.GameImage("Deploy"), 0, 0);
                    break;
                }

            default:
                {
                    SwinGame.ClearScreen();
                    break;
                }
        }

        SwinGame.DrawFramerate(675, 585, GameResources.GameFont("CourierSmall"));
    }

    public static void AddExplosion(int row, int col)
    {
        AddAnimation(row, col, "Splash");
    }

    public static void AddSplash(int row, int col)
    {
        AddAnimation(row, col, "Splash");
    }

    private static List<Sprite> _Animations = new List<Sprite>();

    private static void AddAnimation(int row, int col, string image)
    {
        Sprite s;
        Bitmap imgObj;

        imgObj = GameResources.GameImage(image);
        imgObj.SetCellDetails(40, 40, 3, 3, 7);

        AnimationScript animation;
        animation = SwinGame.LoadAnimationScript("splash.txt");

        s = SwinGame.CreateSprite(imgObj, animation);
        s.X = FIELD_LEFT + col * (CELL_WIDTH + CELL_GAP);
        s.Y = FIELD_TOP + row * (CELL_HEIGHT + CELL_GAP);

        s.StartAnimation("splash");
        _Animations.Add(s);
    }

    public static void UpdateAnimations()
    {
        List<Sprite> ended = new List<Sprite>();
        foreach (Sprite s in _Animations)
        {
            SwinGame.UpdateSprite(s);
            if (s.AnimationHasEnded)
                ended.Add(s);
        }

        foreach (Sprite s in ended)
        {
            _Animations.Remove(s);
            SwinGame.FreeSprite(s);
        }
    }

    public static void DrawAnimations()
    {
        foreach (Sprite s in _Animations)
            SwinGame.DrawSprite(s);
    }

    public static void DrawAnimationSequence()
    {
        int i;
        for (i = 1; i <= ANIMATION_CELLS * FRAMES_PER_CELL; i++)
        {
            UpdateAnimations();
            GameController.DrawScreen();
        }
    }
}
