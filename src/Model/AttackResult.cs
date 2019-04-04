/// <summary>
/// ''' AttackResult gives the result after a shot has been made.
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

public class AttackResult
{
    private ResultOfAttack _Value;
    private Ship _Ship;
    private string _Text;
    private int _Row;
    private int _Column;

    /// <summary>
    ///     ''' The result of the attack
    ///     ''' </summary>
    ///     ''' <value>The result of the attack</value>
    ///     ''' <returns>The result of the attack</returns>
    public ResultOfAttack Value
    {
        get
        {
            return _Value;
        }
    }

    /// <summary>
    ///     ''' The ship, if any, involved in this result
    ///     ''' </summary>
    ///     ''' <value>The ship, if any, involved in this result</value>
    ///     ''' <returns>The ship, if any, involved in this result</returns>
    public Ship Ship
    {
        get
        {
            return _Ship;
        }
    }

    /// <summary>
    ///     ''' A textual description of the result.
    ///     ''' </summary>
    ///     ''' <value>A textual description of the result.</value>
    ///     ''' <returns>A textual description of the result.</returns>
    ///     ''' <remarks>A textual description of the result.</remarks>
    public string Text
    {
        get
        {
            return _Text;
        }
    }

    /// <summary>
    ///     ''' The row where the attack occurred
    ///     ''' </summary>
    public int Row
    {
        get
        {
            return _Row;
        }
    }

    /// <summary>
    ///     ''' The column where the attack occurred
    ///     ''' </summary>
    public int Column
    {
        get
        {
            return _Column;
        }
    }

    /// <summary>
    ///     ''' Set the _Value to the PossibleAttack value
    ///     ''' </summary>
    ///     ''' <param name="value">either hit, miss, destroyed, shotalready</param>
    public AttackResult(ResultOfAttack value, string text, int row, int column)
    {
        _Value = value;
        _Text = text;
        _Ship = null;
        _Row = row;
        _Column = column;
    }

    /// <summary>
    ///     ''' Set the _Value to the PossibleAttack value, and the _Ship to the ship
    ///     ''' </summary>
    ///     ''' <param name="value">either hit, miss, destroyed, shotalready</param>
    ///     ''' <param name="ship">the ship information</param>
    public AttackResult(ResultOfAttack value, Ship ship, string text, int row, int column) : this(value, text, row, column)
    {
        _Ship = ship;
    }

    /// <summary>
    ///     ''' Displays the textual information about the attack
    ///     ''' </summary>
    ///     ''' <returns>The textual information about the attack</returns>
    public override string ToString()
    {
        if (_Ship == null)
            return Text;

        return Text + " " + _Ship.Name;
    }
}
