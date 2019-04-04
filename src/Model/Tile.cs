/// <summary>
/// ''' Tile knows its location on the grid, if it is a ship and if it has been 
/// ''' shot before
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

public class Tile
{
    private readonly int _RowValue;        // the row value of the tile
    private readonly int _ColumnValue;     // the column value of the tile
    private Ship _Ship = null/* TODO Change to default(_) if this is not a reference type */;     // the ship the tile belongs to
    private bool _Shot = false;    // the tile has been shot at

    /// <summary>
    ///     ''' Has the tile been shot?
    ///     ''' </summary>
    ///     ''' <value>indicate if the tile has been shot</value>
    ///     ''' <returns>true if the tile was shot</returns>
    public bool Shot
    {
        get
        {
            return _Shot;
        }
        set
        {
            _Shot = value;
        }
    }

    /// <summary>
    ///     ''' The row of the tile in the grid
    ///     ''' </summary>
    ///     ''' <value>the row index of the tile in the grid</value>
    ///     ''' <returns>the row index of the tile</returns>
    public int Row
    {
        get
        {
            return _RowValue;
        }
    }

    /// <summary>
    ///     ''' The column of the tile in the grid
    ///     ''' </summary>
    ///     ''' <value>the column of the tile in the grid</value>
    ///     ''' <returns>the column of the tile in the grid</returns>
    public int Column
    {
        get
        {
            return _ColumnValue;
        }
    }

    /// <summary>
    ///     ''' Ship allows for a tile to check if there is ship and add a ship to a tile
    ///     ''' </summary>
    public Ship Ship
    {
        get
        {
            return _Ship;
        }
        set
        {
            if (_Ship == null)
            {
                _Ship = value;
                if (value != null)
                    _Ship.AddTile(this);
            }
            else
                throw new InvalidOperationException("There is already a ship at [" + Row + ", " + Column + "]");
        }
    }

    /// <summary>
    ///     ''' The tile constructor will know where it is on the grid, and is its a ship
    ///     ''' </summary>
    ///     ''' <param name="row">the row on the grid</param>
    ///     ''' <param name="col">the col on the grid</param>
    ///     ''' <param name="ship">what ship it is</param>
    public Tile(int row, int col, Ship ship)
    {
        _RowValue = row;
        _ColumnValue = col;
        _Ship = ship;
    }

    /// <summary>
    ///     ''' Clearship will remove the ship from the tile
    ///     ''' </summary>
    public void ClearShip()
    {
        _Ship = null;
    }

    /// <summary>
    ///     ''' View is able to tell the grid what the tile is
    ///     ''' </summary>
    public TileView View
    {
        get
        {
            // if there is no ship in the tile
            if (_Ship == null)
            {
                // and the tile has been hit
                if (_Shot)
                    return TileView.Miss;
                else
                    // and the tile hasn't been hit
                    return TileView.Sea;
            }
            else
                // if there is a ship and it has been hit
                if ((_Shot))
                return TileView.Hit;
            else
                // if there is a ship and it hasn't been hit
                return TileView.Ship;
        }
    }

    /// <summary>
    ///     ''' Shoot allows a tile to be shot at, and if the tile has been hit before
    ///     ''' it will give an error
    ///     ''' </summary>
    internal void Shoot()
    {
        if ((false == Shot))
        {
            Shot = true;
            if (_Ship != null)
                _Ship.Hit();
        }
        else
            throw new ApplicationException("You have already shot this square");
    }
}
