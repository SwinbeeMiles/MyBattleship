/// <summary>
/// ''' The BattleShipsGame controls a big part of the game. It will add the two players
/// ''' to the game and make sure that both players ships are all deployed before starting the game.
/// ''' It also allows players to shoot and swap turns between player. It will also check if players 
/// ''' are destroyed.
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

public class BattleShipsGame
{

    /// <summary>
    ///     ''' The attack delegate type is used to send notifications of the end of an
    ///     ''' attack by a player or the AI.
    ///     ''' </summary>
    ///     ''' <param name="sender">the game sending the notification</param>
    ///     ''' <param name="result">the result of the attack</param>
    public delegate void AttackCompletedHandler(object sender, AttackResult result);

    /// <summary>
    ///     ''' The AttackCompleted event is raised when an attack has completed.
    ///     ''' </summary>
    ///     ''' <remarks>
    ///     ''' This is used by the UI to play sound effects etc.
    ///     ''' </remarks>
    public event AttackCompletedHandler AttackCompleted;

    private Player[] _players = new Player[3];
    private int _playerIndex = 0;

    /// <summary>
    ///     ''' The current player.
    ///     ''' </summary>
    ///     ''' <value>The current player</value>
    ///     ''' <returns>The current player</returns>
    ///     ''' <remarks>This value will switch between the two players as they have their attacks</remarks>
    public Player Player
    {
        get
        {
            return _players[_playerIndex];
        }
    }

    /// <summary>
    ///     ''' AddDeployedPlayer adds both players and will make sure
    ///     ''' that the AI player deploys all ships
    ///     ''' </summary>
    ///     ''' <param name="p"></param>
    public void AddDeployedPlayer(Player p)
    {
        if (_players[0] == null)
            _players[0] = p;
        else if (_players[1] == null)
        {
            _players[1] = p;
            CompleteDeployment();
        }
        else
            throw new ApplicationException("You cannot add another player, the game already has two players.");
    }

    /// <summary>
    ///     ''' Assigns each player the other's grid as the enemy grid. This allows each player
    ///     ''' to examine the details visable on the other's sea grid.
    ///     ''' </summary>
    private void CompleteDeployment()
    {
        _players[0].Enemy = new SeaGridAdapter(_players[1].PlayerGrid);
        _players[1].Enemy = new SeaGridAdapter(_players[0].PlayerGrid);
    }

    /// <summary>
    ///     ''' Shoot will swap between players and check if a player has been killed.
    ///     ''' It also allows the current player to hit on the enemygrid.
    ///     ''' </summary>
    ///     ''' <param name="row">the row fired upon</param>
    ///     ''' <param name="col">the column fired upon</param>
    ///     ''' <returns>The result of the attack</returns>
    public AttackResult Shoot(int row, int col)
    {
        AttackResult newAttack;
        int otherPlayer = (_playerIndex + 1) % 2;

        newAttack = Player.Shoot(row, col);

        // Will exit the game when all players ships are destroyed
        if (_players[otherPlayer].IsDestroyed)
            newAttack = new AttackResult(ResultOfAttack.GameOver, newAttack.Ship, newAttack.Text, row, col);

        AttackCompleted?.Invoke(this, newAttack);

        // change player if the last hit was a miss
        if (newAttack.Value == ResultOfAttack.Miss)
            _playerIndex = otherPlayer;

        return newAttack;
    }
}
