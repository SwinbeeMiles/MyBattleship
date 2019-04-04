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

/// ''' The battle phase is handled by the DiscoveryController.

/// ''' </summary>
static class DiscoveryController
{

    /// <summary>
    ///     ''' Handles input during the discovery phase of the game.
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' Escape opens the game menu. Clicking the mouse will
    ///     ''' attack a location.
    ///     ''' </remarks>
    public static void HandleDiscoveryInput()
    {
        if (SwinGame.KeyTyped(KeyCode.VK_ESCAPE))
            AddNewState(GameState.ViewingGameMenu);

        if (SwinGame.MouseClicked(MouseButton.LeftButton))
            DoAttack();
    }

    /// <summary>
    ///     ''' Attack the location that the mouse if over.
    ///     ''' </summary>
    private static void DoAttack()
    {
        Point2D mouse;

        mouse = SwinGame.MousePosition();

        // Calculate the row/col clicked
        int row, col;
        row = Convert.ToInt32(Math.Floor((mouse.Y - FIELD_TOP) / (double)(CELL_HEIGHT + CELL_GAP)));
        col = Convert.ToInt32(Math.Floor((mouse.X - FIELD_LEFT) / (double)(CELL_WIDTH + CELL_GAP)));

        if (row >= 0 & row < HumanPlayer.EnemyGrid.Height)
        {
            if (col >= 0 & col < HumanPlayer.EnemyGrid.Width)
                Attack(row, col);
        }
    }

    /// <summary>
    ///     ''' Draws the game during the attack phase.
    ///     ''' </summary>s
    public static void DrawDiscovery()
    {
        const int SCORES_LEFT = 172;
        const int SHOTS_TOP = 157;
        const int HITS_TOP = 206;
        const int SPLASH_TOP = 256;

        if ((SwinGame.KeyDown(KeyCode.VK_LSHIFT) | SwinGame.KeyDown(KeyCode.VK_RSHIFT)) & SwinGame.KeyDown(KeyCode.VK_C))
            DrawField(HumanPlayer.EnemyGrid, ComputerPlayer, true);
        else
            DrawField(HumanPlayer.EnemyGrid, ComputerPlayer, false);

        DrawSmallField(HumanPlayer.PlayerGrid, HumanPlayer);
        DrawMessage();

        SwinGame.DrawText(HumanPlayer.Shots.ToString(), Color.White, GameFont("Menu"), SCORES_LEFT, SHOTS_TOP);
        SwinGame.DrawText(HumanPlayer.Hits.ToString(), Color.White, GameFont("Menu"), SCORES_LEFT, HITS_TOP);
        SwinGame.DrawText(HumanPlayer.Missed.ToString(), Color.White, GameFont("Menu"), SCORES_LEFT, SPLASH_TOP);
    }
}
