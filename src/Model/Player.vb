''' <summary>
''' Player has its own _PlayerGrid, and can see an _EnemyGrid, it can also check if
''' all ships are deployed and if all ships are detroyed. A Player can also attach.
''' </summary>
Public Class Player : Implements IEnumerable(Of Ship)
    Protected Shared _Random As New Random()

    Private _Ships As New Dictionary(Of ShipName, Ship)()
    Private _playerGrid As New SeaGrid(_Ships)
    Private _enemyGrid As ISeaGrid
    Protected _game As BattleShipsGame

    Private _shots As Integer
    Private _hits As Integer
    Private _misses As Integer

    ''' <summary>
    ''' Returns the game that the player is part of.
    ''' </summary>
    ''' <value>The game</value>
    ''' <returns>The game that the player is playing</returns>
    Public Property Game() As BattleShipsGame
        Get
            Return _game
        End Get
        Set(ByVal value As BattleShipsGame)
            _game = value
        End Set
    End Property

    ''' <summary>
    ''' Sets the grid of the enemy player
    ''' </summary>
    ''' <value>The enemy's sea grid</value>
    Public WriteOnly Property Enemy() As ISeaGrid
        Set(ByVal value As ISeaGrid)
            _enemyGrid = value
        End Set
    End Property

    Public Sub New(ByVal controller As BattleShipsGame)
        _game = controller

        'for each ship add the ships name so the seagrid knows about them
        For Each name As ShipName In [Enum].GetValues(GetType(ShipName))
            If name <> ShipName.None Then
                _Ships.Add(name, New Ship(name))
            End If
        Next

        RandomizeDeployment()
    End Sub

    ''' <summary>
    ''' The EnemyGrid is a ISeaGrid because you shouldn't be allowed to see the enemies ships
    ''' </summary>
    Public Property EnemyGrid() As ISeaGrid
        Get
            Return _enemyGrid
        End Get
        Set(ByVal value As ISeaGrid)
            _enemyGrid = value
        End Set
    End Property

    ''' <summary>
    ''' The PlayerGrid is just a normal SeaGrid where the players ships can be deployed and seen
    ''' </summary>
    Public ReadOnly Property PlayerGrid() As SeaGrid
        Get
            Return _playerGrid
        End Get
    End Property

    ''' <summary>
    ''' ReadyToDeploy returns true if all ships are deployed
    ''' </summary>
    Public ReadOnly Property ReadyToDeploy() As Boolean
        Get
            Return _playerGrid.AllDeployed
        End Get
    End Property

    Public ReadOnly Property IsDestroyed() As Boolean
        Get
            'Check if all ships are destroyed... -1 for the none ship
            Return _playerGrid.ShipsKilled = [Enum].GetValues(GetType(ShipName)).Length - 1
        End Get
    End Property

    ''' <summary>
    ''' Returns the Player's ship with the given name.
    ''' </summary>
    ''' <param name="name">the name of the ship to return</param>
    ''' <value>The ship</value>
    ''' <returns>The ship with the indicated name</returns>
    ''' <remarks>The none ship returns nothing/null</remarks>
    Public ReadOnly Property Ship(ByVal name As ShipName) As Ship
        Get
            If name = ShipName.None Then Return Nothing

            Return _Ships.Item(name)
        End Get
    End Property

    ''' <summary>
    ''' The number of shots the player has made
    ''' </summary>
    ''' <value>shots taken</value>
    ''' <returns>teh number of shots taken</returns>
    Public ReadOnly Property Shots() As Integer
        Get
            Return _shots
        End Get
    End Property

    Public ReadOnly Property Hits() As Integer
        Get
            Return _hits
        End Get
    End Property

    ''' <summary>
    ''' Total number of shots that missed
    ''' </summary>
    ''' <value>miss count</value>
    ''' <returns>the number of shots that have missed ships</returns>
    Public ReadOnly Property Missed() As Integer
        Get
            Return _misses
        End Get
    End Property

    Public ReadOnly Property Score() As Integer
        Get
            If IsDestroyed Then
                Return 0
            Else
                Return (Hits * 12) - Shots - (PlayerGrid.ShipsKilled * 20)
            End If
        End Get
    End Property

    ''' <summary>
    ''' Makes it possible to enumerate over the ships the player
    ''' has.
    ''' </summary>
    ''' <returns>A Ship enumerator</returns>
    Public Function GetShipEnumerator() As IEnumerator(Of Ship) Implements IEnumerable(Of Ship).GetEnumerator
        Dim result(_Ships.Values.Count) As Ship
        _Ships.Values.CopyTo(result, 0)
        Dim lst As New List(Of Ship)
        lst.AddRange(result)

        Return lst.GetEnumerator()
    End Function

    ''' <summary>
    ''' Makes it possible to enumerate over the ships the player
    ''' has.
    ''' </summary>
    ''' <returns>A Ship enumerator</returns>
    Public Function GetEnumerator() As IEnumerator Implements IEnumerable.GetEnumerator
        Dim result(_Ships.Values.Count) As Ship
        _Ships.Values.CopyTo(result, 0)
        Dim lst As New List(Of Ship)
        lst.AddRange(result)

        Return lst.GetEnumerator()
    End Function

    ''' <summary>
    ''' Vitual Attack allows the player to shoot
    ''' </summary>
    Public Overridable Function Attack() As AttackResult
        'human does nothing here...
        Return Nothing
    End Function

    ''' <summary>
    ''' Shoot at a given row/column
    ''' </summary>
    ''' <param name="row">the row to attack</param>
    ''' <param name="col">the column to attack</param>
    ''' <returns>the result of the attack</returns>
    Friend Function Shoot(ByVal row As Integer, ByVal col As Integer) As AttackResult
        _shots += 1
        Dim result As AttackResult
        result = EnemyGrid.HitTile(row, col)

        Select Case result.Value
            Case ResultOfAttack.Destroyed, ResultOfAttack.Hit
                _hits += 1
            Case ResultOfAttack.Miss
                _misses += 1
        End Select

        Return result
    End Function

    Public Overridable Sub RandomizeDeployment()
        Dim placementSuccessful As Boolean
        Dim heading As Direction

        'for each ship to deploy in shipist
        For Each shipToPlace As ShipName In [Enum].GetValues(GetType(ShipName))

            If shipToPlace = ShipName.None Then Continue For

            placementSuccessful = False

            'generate random position until the ship can be placed
            Do
                Dim dir As Integer = _Random.Next(2)
                Dim x As Integer = _Random.Next(0, 11)
                Dim y As Integer = _Random.Next(0, 11)


                If dir = 0 Then
                    heading = Direction.UpDown
                Else
                    heading = Direction.LeftRight
                End If

                'try to place ship, if position unplaceable, generate new coordinates
                Try
                    PlayerGrid.MoveShip(x, y, shipToPlace, heading)
                    placementSuccessful = True
                Catch
                    placementSuccessful = False
                End Try
            Loop While Not placementSuccessful
        Next
    End Sub
End Class
