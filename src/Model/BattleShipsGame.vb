''' <summary>
''' The BattleShipsGame controls a big part of the game. It will add the two players
''' to the game and make sure that both players ships are all deployed before starting the game.
''' It also allows players to shoot and swap turns between player. It will also check if players 
''' are destroyed.
''' </summary>
Public Class BattleShipsGame

    ''' <summary>
    ''' The attack delegate type is used to send notifications of the end of an
    ''' attack by a player or the AI.
    ''' </summary>
    ''' <param name="sender">the game sending the notification</param>
    ''' <param name="result">the result of the attack</param>
    Public Delegate Sub AttackCompletedHandler(ByVal sender As Object, ByVal result As AttackResult)

    ''' <summary>
    ''' The AttackCompleted event is raised when an attack has completed.
    ''' </summary>
    ''' <remarks>
    ''' This is used by the UI to play sound effects etc.
    ''' </remarks>
    Public Event AttackCompleted As AttackCompletedHandler

    Private _players(2) As Player
    Private _playerIndex As Integer = 0

    ''' <summary>
    ''' The current player.
    ''' </summary>
    ''' <value>The current player</value>
    ''' <returns>The current player</returns>
    ''' <remarks>This value will switch between the two players as they have their attacks</remarks>
    Public ReadOnly Property Player() As Player
        Get
            Return _players(_playerIndex)
        End Get
    End Property

    ''' <summary>
    ''' AddDeployedPlayer adds both players and will make sure
    ''' that the AI player deploys all ships
    ''' </summary>
    ''' <param name="p"></param>
    Public Sub AddDeployedPlayer(ByVal p As Player)
        If _players(0) Is Nothing Then
            _players(0) = p
        ElseIf _players(1) Is Nothing Then
            _players(1) = p
            CompleteDeployment()
        Else
            Throw New ApplicationException("You cannot add another player, the game already has two players.")
        End If
    End Sub

    ''' <summary>
    ''' Assigns each player the other's grid as the enemy grid. This allows each player
    ''' to examine the details visable on the other's sea grid.
    ''' </summary>
    Private Sub CompleteDeployment()
        _players(0).Enemy = New SeaGridAdapter(_players(1).PlayerGrid)
        _players(1).Enemy = New SeaGridAdapter(_players(0).PlayerGrid)
    End Sub

    ''' <summary>
    ''' Shoot will swap between players and check if a player has been killed.
    ''' It also allows the current player to hit on the enemygrid.
    ''' </summary>
    ''' <param name="row">the row fired upon</param>
    ''' <param name="col">the column fired upon</param>
    ''' <returns>The result of the attack</returns>
    Public Function Shoot(ByVal row As Integer, ByVal col As Integer) As AttackResult
        Dim newAttack As AttackResult
        Dim otherPlayer As Integer = (_playerIndex + 1) Mod 2

        newAttack = Player.Shoot(row, col)

        'Will exit the game when all players ships are destroyed
        If _players(otherPlayer).IsDestroyed Then
            newAttack = New AttackResult(ResultOfAttack.GameOver, newAttack.Ship, newAttack.Text, row, col)
        End If

        RaiseEvent AttackCompleted(Me, newAttack)

        'change player if the last hit was a miss
        If newAttack.Value = ResultOfAttack.Miss Then
            _playerIndex = otherPlayer
        End If

        Return newAttack
    End Function
End Class
