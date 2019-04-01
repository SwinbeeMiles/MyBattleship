/// <summary>
/// ''' The AIMediumPlayer is a type of AIPlayer where it will try and destroy a ship
/// ''' if it has found a ship
/// ''' </summary>
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

public class AIMediumPlayer : AIPlayer
{
    /// <summary>
    ///     ''' Private enumarator for AI states. currently there are two states,
    ///     ''' the AI can be searching for a ship, or if it has found a ship it will
    ///     ''' target the same ship
    ///     ''' </summary>
    private enum AIStates
    {
        Searching,
        TargetingShip
    }

    private AIStates _CurrentState = AIStates.Searching;
    private Stack<Location> _Targets = new Stack<Location>();

    public AIMediumPlayer(BattleShipsGame controller) : base(controller)
    {
    }

    /// <summary>
    ///     ''' GenerateCoordinates should generate random shooting coordinates
    ///     ''' only when it has not found a ship, or has destroyed a ship and 
    ///     ''' needs new shooting coordinates
    ///     ''' </summary>
    ///     ''' <param name="row">the generated row</param>
    ///     ''' <param name="column">the generated column</param>
    protected override void GenerateCoords(ref int row, ref int column)
    {
        do
        {
            // check which state the AI is in and uppon that choose which coordinate generation
            // method will be used.
            switch (_CurrentState)
            {
                case AIStates.Searching:
                    {
                        SearchCoords(ref row, ref column);
                        break;
                    }

                case AIStates.TargetingShip:
                    {
                        TargetCoords(ref row, ref column);
                        break;
                    }

                default:
                    {
                        throw new ApplicationException("AI has gone in an imvalid state");
                        break;
                    }
            }
        }
        while ((row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width || EnemyGrid.Item(row, column) != TileView.Sea)); // while inside the grid and not a sea tile do the search
    }

    /// <summary>
    ///     ''' TargetCoords is used when a ship has been hit and it will try and destroy
    ///     ''' this ship
    ///     ''' </summary>
    ///     ''' <param name="row">row generated around the hit tile</param>
    ///     ''' <param name="column">column generated around the hit tile</param>
    private void TargetCoords(ref int row, ref int column)
    {
        Location l = _Targets.Pop();

        if ((_Targets.Count == 0))
            _CurrentState = AIStates.Searching;
        row = l.Row;
        column = l.Column;
    }

    /// <summary>
    ///     ''' SearchCoords will randomly generate shots within the grid as long as its not hit that tile already
    ///     ''' </summary>
    ///     ''' <param name="row">the generated row</param>
    ///     ''' <param name="column">the generated column</param>
    private void SearchCoords(ref int row, ref int column)
    {
        row = _Random.Next(0, EnemyGrid.Height);
        column = _Random.Next(0, EnemyGrid.Width);
    }

    /// <summary>
    ///     ''' ProcessShot will be called uppon when a ship is found.
    ///     ''' It will create a stack with targets it will try to hit. These targets
    ///     ''' will be around the tile that has been hit.
    ///     ''' </summary>
    ///     ''' <param name="row">the row it needs to process</param>
    ///     ''' <param name="col">the column it needs to process</param>
    ///     ''' <param name="result">the result og the last shot (should be hit)</param>
    protected override void ProcessShot(int row, int col, AttackResult result)
    {
        if (result.Value == ResultOfAttack.Hit)
        {
            _CurrentState = AIStates.TargetingShip;
            AddTarget(row - 1, col);
            AddTarget(row, col - 1);
            AddTarget(row + 1, col);
            AddTarget(row, col + 1);
        }
        else if (result.Value == ResultOfAttack.ShotAlready)
            throw new ApplicationException("Error in AI");
    }

    /// <summary>
    ///     ''' AddTarget will add the targets it will shoot onto a stack
    ///     ''' </summary>
    ///     ''' <param name="row">the row of the targets location</param>
    ///     ''' <param name="column">the column of the targets location</param>
    private void AddTarget(int row, int column)
    {
        if (row >= 0 && column >= 0 && row < EnemyGrid.Height && column < EnemyGrid.Width && EnemyGrid.Item(row, column) == TileView.Sea)

            _Targets.Push(new Location(row, column));
    }
}
