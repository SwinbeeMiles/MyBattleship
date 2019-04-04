/// <summary>
/// ''' The ISeaGrid defines the read only interface of a Grid. This
/// ''' allows each player to see and attack their opponents grid.
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

public interface ISeaGrid
{
    int Width { get; }

    int Height { get; }

    /// <summary>
    ///     ''' Indicates that the grid has changed.
    ///     ''' </summary>
    event EventHandler Changed;

    /// <summary>
    ///     ''' Provides access to the given row/column
    ///     ''' </summary>
    ///     ''' <param name="row">the row to access</param>
    ///     ''' <param name="column">the column to access</param>
    ///     ''' <value>what the player can see at that location</value>
    ///     ''' <returns>what the player can see at that location</returns>
    TileView Item { get; }

    /// <summary>
    ///     ''' Mark the indicated tile as shot.
    ///     ''' </summary>
    ///     ''' <param name="row">the row of the tile</param>
    ///     ''' <param name="col">the column of the tile</param>
    ///     ''' <returns>the result of the attack</returns>
    AttackResult HitTile(int row, int col);
}
