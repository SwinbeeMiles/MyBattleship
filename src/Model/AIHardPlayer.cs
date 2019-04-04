/// <summary>
/// ''' AIHardPlayer is a type of player. This AI will know directions of ships
/// ''' when it has found 2 ship tiles and will try to destroy that ship. If that ship
/// ''' is not destroyed it will shoot the other way. Ship still not destroyed, then
/// ''' the AI knows it has hit multiple ships. Then will try to destoy all around tiles
/// ''' that have been hit.
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

public class AIHardPlayer : AIPlayer
{

    /// <summary>
    ///     ''' Target allows the AI to know more things, for example the source of a
    ///     ''' shot target
    ///     ''' </summary>
    protected class Target
    {
        private readonly Location _ShotAt;
        private readonly Location _Source;

        /// <summary>
        ///         ''' The target shot at
        ///         ''' </summary>
        ///         ''' <value>The target shot at</value>
        ///         ''' <returns>The target shot at</returns>
        public Location ShotAt
        {
            get
            {
                return _ShotAt;
            }
        }

        /// <summary>
        ///         ''' The source that added this location as a target.
        ///         ''' </summary>
        ///         ''' <value>The source that added this location as a target.</value>
        ///         ''' <returns>The source that added this location as a target.</returns>
        public Location Source
        {
            get
            {
                return _Source;
            }
        }

        internal Target(Location shootat, Location source)
        {
            _ShotAt = shootat;
            _Source = source;
        }

        /// <summary>
        ///         ''' If source shot and shootat shot are on the same row then 
        ///         ''' give a boolean true
        ///         ''' </summary>
        public bool SameRow
        {
            get
            {
                return _ShotAt.Row == _Source.Row;
            }
        }

        /// <summary>
        ///         ''' If source shot and shootat shot are on the same column then 
        ///         ''' give a boolean true 
        ///         ''' </summary>
        public bool SameColumn
        {
            get
            {
                return _ShotAt.Column == _Source.Column;
            }
        }
    }

    /// <summary>
    ///     ''' Private enumarator for AI states. currently there are two states,
    ///     ''' the AI can be searching for a ship, or if it has found a ship it will
    ///     ''' target the same ship
    ///     ''' </summary>
    private enum AIStates
    {
        /// <summary>
        ///         ''' The AI is searching for its next target
        ///         ''' </summary>
        Searching,

        /// <summary>
        ///         ''' The AI is trying to target a ship
        ///         ''' </summary>
        TargetingShip,

        /// <summary>
        ///         ''' The AI is locked onto a ship
        ///         ''' </summary>
        HittingShip
    }

    private AIStates _CurrentState = AIStates.Searching;
    private Stack<Target> _Targets = new Stack<Target>();
    private List<Target> _LastHit = new List<Target>();
    private Target _CurrentTarget;

    public AIHardPlayer(BattleShipsGame game) : base(game)
    {
    }

    /// <summary>
    ///     ''' GenerateCoords will call upon the right methods to generate the appropriate shooting
    ///     ''' coordinates
    ///     ''' </summary>
    ///     ''' <param name="row">the row that will be shot at</param>
    ///     ''' <param name="column">the column that will be shot at</param>
    protected override void GenerateCoords(ref int row, ref int column)
    {
        do
        {
            _CurrentTarget = null;

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
                case AIStates.HittingShip:
                    {
                        TargetCoords(ref row, ref column);
                        break;
                    }

                default:
                    {
                        throw new ApplicationException("AI has gone in an invalid state");
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
        Target t;
        t = _Targets.Pop();

        row = t.ShotAt.Row;
        column = t.ShotAt.Column;
        _CurrentTarget = t;
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
        _CurrentTarget = new Target(new Location(row, column), null/* TODO Change to default(_) if this is not a reference type */);
    }

    /// <summary>
    ///     ''' ProcessShot is able to process each shot that is made and call the right methods belonging
    ///     ''' to that shot. For example, if its a miss = do nothing, if it's a hit = process that hit location
    ///     ''' </summary>
    ///     ''' <param name="row">the row that was shot at</param>
    ///     ''' <param name="col">the column that was shot at</param>
    ///     ''' <param name="result">the result from that hit</param>
    protected override void ProcessShot(int row, int col, AttackResult result)
    {
        switch (result.Value)
        {
            case object _ when ResultOfAttack.Miss:
                {
                    _CurrentTarget = null;
                    break;
                }

            case object _ when ResultOfAttack.Hit:
                {
                    ProcessHit(row, col);
                    break;
                }

            case object _ when ResultOfAttack.Destroyed:
                {
                    ProcessDestroy(row, col, result.Ship);
                    break;
                }

            case object _ when ResultOfAttack.ShotAlready:
                {
                    throw new ApplicationException("Error in AI");
                    break;
                }
        }

        if (_Targets.Count == 0)
            _CurrentState = AIStates.Searching;
    }

    /// <summary>
    ///     ''' ProcessDetroy is able to process the destroyed ships targets and remove _LastHit targets.
    ///     ''' It will also call RemoveShotsAround to remove targets that it was going to shoot at
    ///     ''' </summary>
    ///     ''' <param name="row">the row that was shot at and destroyed</param>
    ///     ''' <param name="col">the row that was shot at and destroyed</param>
    ///     ''' <param name="ship">the row that was shot at and destroyed</param>
    private void ProcessDestroy(int row, int col, Ship ship)
    {
        bool foundOriginal;
        Location source;
        Target current;
        current = _CurrentTarget;

        foundOriginal = false;

        // i = 1, as we dont have targets from the current hit...
        int i;
        for (i = 1; i <= ship.Hits - 1; i++)
        {
            if (!foundOriginal)
            {
                source = current.Source;
                // Source is nnothing if the ship was originally hit in
                // the middle. This then searched forward, rather than
                // backward through the list of targets
                if (source == null)
                {
                    source = current.ShotAt;
                    foundOriginal = true;
                }
            }
            else
                source = current.ShotAt;

            // find the source in _LastHit
            foreach (Target t in _LastHit)
            {
                if ((!foundOriginal && t.ShotAt == source) || (foundOriginal & t.Source == source))
                {
                    current = t;
                    _LastHit.Remove(t);
                    break;
                }
            }

            RemoveShotsAround(current.ShotAt);
        }
    }

    /// <summary>
    ///     ''' RemoveShotsAround will remove targets that belong to the destroyed ship by checking if 
    ///     ''' the source of the targets belong to the destroyed ship. If they don't put them on a new stack.
    ///     ''' Then clear the targets stack and move all the targets that still need to be shot at back 
    ///     ''' onto the targets stack
    ///     ''' </summary>
    ///     ''' <param name="toRemove"></param>
    private void RemoveShotsAround(Location toRemove)
    {
        Stack<Target> newStack = new Stack<Target>();  // create a new stack

        // check all targets in the _Targets stack
        foreach (Target t in _Targets)
        {

            // if the source of the target does not belong to the destroyed ship put them on the newStack
            if (t.Source != toRemove)
                newStack.Push(t);
        }

        _Targets.Clear();   // clear the _Targets stack

        // for all the targets in the newStack, move them back onto the _Targets stack
        foreach (Target t in newStack)
            _Targets.Push(t);

        // if the _Targets stack is 0 then change the AI's state back to searching
        if (_Targets.Count == 0)
            _CurrentState = AIStates.Searching;
    }

    /// <summary>
    ///     ''' ProcessHit gets the last hit location coordinates and will ask AddTarget to
    ///     ''' create targets around that location by calling the method four times each time with
    ///     ''' a new location around the last hit location.
    ///     ''' It will then set the state of the AI and if it's not Searching or targetingShip then 
    ///     ''' start ReOrderTargets.
    ///     ''' </summary>
    ///     ''' <param name="row"></param>
    ///     ''' <param name="col"></param>
    private void ProcessHit(int row, int col)
    {
        _LastHit.Add(_CurrentTarget);

        // Uses _CurrentTarget as the source
        AddTarget(row - 1, col);
        AddTarget(row, col - 1);
        AddTarget(row + 1, col);
        AddTarget(row, col + 1);

        if (_CurrentState == AIStates.Searching)
            _CurrentState = AIStates.TargetingShip;
        else
        {
            // either targetting or hitting... both are the same here
            _CurrentState = AIStates.HittingShip;

            ReOrderTargets();
        }
    }

    /// <summary>
    ///     ''' ReOrderTargets will optimise the targeting by re-orderin the stack that the targets are in.
    ///     ''' By putting the most important targets at the top they are the ones that will be shot at first.
    ///     ''' </summary>
    private void ReOrderTargets()
    {

        // if the ship is lying on the same row, call MoveToTopOfStack to optimise on the row
        if (_CurrentTarget.SameRow)
            MoveToTopOfStack(_CurrentTarget.ShotAt.Row, -1);
        else if (_CurrentTarget.SameColumn)
            // else if the ship is lying on the same column, call MoveToTopOfStack to optimise on the column
            MoveToTopOfStack(-1, _CurrentTarget.ShotAt.Column);
    }

    /// <summary>
    ///     ''' MoveToTopOfStack will re-order the stack by checkin the coordinates of each target
    ///     ''' If they have the right column or row values it will be moved to the _Match stack else 
    ///     ''' put it on the _NoMatch stack. Then move all the targets from the _NoMatch stack back on the 
    ///     ''' _Targets stack, these will be at the bottom making them less important. The move all the
    ///     ''' targets from the _Match stack on the _Targets stack, these will be on the top and will there
    ///     ''' for be shot at first
    ///     ''' </summary>
    ///     ''' <param name="row">the row of the optimisation</param>
    ///     ''' <param name="column">the column of the optimisation</param>
    private void MoveToTopOfStack(int row, int column)
    {
        Stack<Target> _NoMatch = new Stack<Target>();
        Stack<Target> _Match = new Stack<Target>();

        Target current;

        while (_Targets.Count > 0)
        {
            current = _Targets.Pop();
            if (current.ShotAt.Row == row || current.ShotAt.Column == column)
                _Match.Push(current);
            else
                _NoMatch.Push(current);
        }

        foreach (Target t in _NoMatch)
            _Targets.Push(t);
        foreach (Target t in _Match)
            _Targets.Push(t);
    }

    /// <summary>
    ///     ''' AddTarget will add the targets it will shoot onto a stack
    ///     ''' </summary>
    ///     ''' <param name="row">the row of the targets location</param>
    ///     ''' <param name="column">the column of the targets location</param>
    private void AddTarget(int row, int column)
    {
        if ((row >= 0 && column >= 0 && row < EnemyGrid.Height && column < EnemyGrid.Width && EnemyGrid.Item(row, column) == TileView.Sea))

            _Targets.Push(new Target(new Location(row, column), _CurrentTarget.ShotAt));
    }
}
