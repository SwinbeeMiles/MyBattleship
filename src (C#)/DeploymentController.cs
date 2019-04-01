using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using SwinGameSDK;

/// <summary>

/// ''' The DeploymentController controls the players actions

/// ''' during the deployment phase.

/// ''' </summary>
static class DeploymentController
{
    private const static int SHIPS_TOP = 98;
    private const static int SHIPS_LEFT = 20;
    private const static int SHIPS_HEIGHT = 90;
    private const static int SHIPS_WIDTH = 300;

    private const static int TOP_BUTTONS_TOP = 72;
    private const static int TOP_BUTTONS_HEIGHT = 46;

    private const static int PLAY_BUTTON_LEFT = 693;
    private const static int PLAY_BUTTON_WIDTH = 80;

    private const static int UP_DOWN_BUTTON_LEFT = 410;
    private const static int LEFT_RIGHT_BUTTON_LEFT = 350;

    private const static int RANDOM_BUTTON_LEFT = 547;
    private const static int RANDOM_BUTTON_WIDTH = 51;

    private const static int DIR_BUTTONS_WIDTH = 47;

    private const static int TEXT_OFFSET = 5;

    private static Direction _currentDirection = Direction.UpDown;
    private static ShipName _selectedShip = ShipName.Tug;

    /// <summary>
    ///     ''' Handles user input for the Deployment phase of the game.
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Involves selecting the ships, deloying ships, changing the direction
    ///     ''' of the ships to add, randomising deployment, end then ending
    ///     ''' deployment
    ///     ''' </remarks>
    public static void HandleDeploymentInput()
    {
        if (SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
            AddNewState(GameState.ViewingGameMenu);

        if (SwinGame.KeyTyped(KeyCode.VK_UP) | SwinGame.KeyTyped(KeyCode.VK_DOWN))
            _currentDirection = Direction.UpDown;
        if (SwinGame.KeyTyped(KeyCode.VK_LEFT) | SwinGame.KeyTyped(KeyCode.VK_RIGHT))
            _currentDirection = Direction.LeftRight;

        if (SwinGame.KeyTyped(KeyCode.VK_R))
            HumanPlayer.RandomizeDeployment();

        if (SwinGame.MouseClicked(MouseButton.LeftButton))
        {
            ShipName selected;
            selected = GetShipMouseIsOver();
            if (selected != ShipName.None)
                _selectedShip = selected;
            else
                DoDeployClick();

            if (HumanPlayer.ReadyToDeploy & IsMouseInRectangle(PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP, PLAY_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
                EndDeployment();
            else if (IsMouseInRectangle(UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
                _currentDirection = Direction.LeftRight;
            else if (IsMouseInRectangle(LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT))
                _currentDirection = Direction.LeftRight;
            else if (IsMouseInRectangle(RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP, RANDOM_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT))
                HumanPlayer.RandomizeDeployment();
        }
    }

    /// <summary>
    ///     ''' The user has clicked somewhere on the screen, check if its is a deployment and deploy
    ///     ''' the current ship if that is the case.
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' If the click is in the grid it deploys to the selected location
    ///     ''' with the indicated direction
    ///     ''' </remarks>
    private static void DoDeployClick()
    {
        Point2D mouse;

        mouse = SwinGame.MousePosition();

        // Calculate the row/col clicked
        int row, col;
        row = Convert.ToInt32(Math.Floor((mouse.Y) / (double)(CELL_HEIGHT + CELL_GAP)));
        col = Convert.ToInt32(Math.Floor((mouse.X - FIELD_LEFT) / (double)(CELL_WIDTH + CELL_GAP)));

        if (row >= 0 & row < HumanPlayer.PlayerGrid.Height)
        {
            if (col >= 0 & col < HumanPlayer.PlayerGrid.Width)
            {
                // if in the area try to deploy
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
    ///     ''' Draws the deployment screen showing the field and the ships
    ///     ''' that the player can deploy.
    ///     ''' </summary>
    public static void DrawDeployment()
    {
        DrawField(HumanPlayer.PlayerGrid, HumanPlayer, true);

        // Draw the Left/Right and Up/Down buttons
        if (_currentDirection == Direction.LeftRight)
            SwinGame.DrawBitmap(GameImage("LeftRightButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);
        else
            SwinGame.DrawBitmap(GameImage("UpDownButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP);

        // DrawShips
        foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
        {
            int i;
            i = Int(sn) - 1;
            if (i >= 0)
            {
                if (sn == _selectedShip)
                    SwinGame.DrawBitmap(GameImage("SelectedShip"), SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT);
            }
        }

        if (HumanPlayer.ReadyToDeploy)
            SwinGame.DrawBitmap(GameImage("PlayButton"), PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP);

        SwinGame.DrawBitmap(GameImage("RandomButton"), RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP);

        DrawMessage();
    }

    /// <summary>
    ///     ''' Gets the ship that the mouse is currently over in the selection panel.
    ///     ''' </summary>
    ///     ''' <returns>The ship selected or none</returns>
    private static ShipName GetShipMouseIsOver()
    {
        foreach (ShipName sn in Enum.GetValues(typeof(ShipName)))
        {
            int i;
            i = Int(sn) - 1;

            if (IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT))
                return sn;
        }

        return ShipName.None;
    }
}
