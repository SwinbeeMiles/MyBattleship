
''' <summary>
''' The AIMediumPlayer is a type of AIPlayer where it will try and destroy a ship
''' if it has found a ship
''' </summary>
Public Class AIMediumPlayer : Inherits AIPlayer
    ''' <summary>
    ''' Private enumarator for AI states. currently there are two states,
    ''' the AI can be searching for a ship, or if it has found a ship it will
    ''' target the same ship
    ''' </summary>
    Private Enum AIStates
        Searching
        TargetingShip
    End Enum

    Private _CurrentState As AIStates = AIStates.Searching
    Private _Targets As New Stack(Of Location)()

    Public Sub New(ByVal controller As BattleShipsGame)
        MyBase.New(controller)
    End Sub

    ''' <summary>
    ''' GenerateCoordinates should generate random shooting coordinates
    ''' only when it has not found a ship, or has destroyed a ship and 
    ''' needs new shooting coordinates
    ''' </summary>
    ''' <param name="row">the generated row</param>
    ''' <param name="column">the generated column</param>
    Protected Overrides Sub GenerateCoords(ByRef row As Integer, ByRef column As Integer)
        Do
            'check which state the AI is in and uppon that choose which coordinate generation
            'method will be used.
            Select Case _CurrentState
                Case AIStates.Searching
                    SearchCoords(row, column)
                Case AIStates.TargetingShip
                    TargetCoords(row, column)
                Case Else
                    Throw New ApplicationException("AI has gone in an imvalid state")
            End Select
        Loop While (row < 0 OrElse column < 0 _
            OrElse row >= EnemyGrid.Height _
            OrElse column >= EnemyGrid.Width _
            OrElse EnemyGrid.Item(row, column) <> TileView.Sea) 'while inside the grid and not a sea tile do the search
    End Sub

    ''' <summary>
    ''' TargetCoords is used when a ship has been hit and it will try and destroy
    ''' this ship
    ''' </summary>
    ''' <param name="row">row generated around the hit tile</param>
    ''' <param name="column">column generated around the hit tile</param>
    Private Sub TargetCoords(ByRef row As Integer, ByRef column As Integer)
        Dim l As Location = _Targets.Pop()

        If (_Targets.Count = 0) Then _CurrentState = AIStates.Searching
        row = l.Row
        column = l.Column
    End Sub

    ''' <summary>
    ''' SearchCoords will randomly generate shots within the grid as long as its not hit that tile already
    ''' </summary>
    ''' <param name="row">the generated row</param>
    ''' <param name="column">the generated column</param>
    Private Sub SearchCoords(ByRef row As Integer, ByRef column As Integer)
        row = _Random.Next(0, EnemyGrid.Height)
        column = _Random.Next(0, EnemyGrid.Width)
    End Sub

    ''' <summary>
    ''' ProcessShot will be called uppon when a ship is found.
    ''' It will create a stack with targets it will try to hit. These targets
    ''' will be around the tile that has been hit.
    ''' </summary>
    ''' <param name="row">the row it needs to process</param>
    ''' <param name="col">the column it needs to process</param>
    ''' <param name="result">the result og the last shot (should be hit)</param>
    Protected Overrides Sub ProcessShot(ByVal row As Integer, ByVal col As Integer, ByVal result As AttackResult)

        If result.Value = ResultOfAttack.Hit Then
            _CurrentState = AIStates.TargetingShip
            AddTarget(row - 1, col)
            AddTarget(row, col - 1)
            AddTarget(row + 1, col)
            AddTarget(row, col + 1)
        ElseIf result.Value = ResultOfAttack.ShotAlready Then
            Throw New ApplicationException("Error in AI")
        End If
    End Sub

    ''' <summary>
    ''' AddTarget will add the targets it will shoot onto a stack
    ''' </summary>
    ''' <param name="row">the row of the targets location</param>
    ''' <param name="column">the column of the targets location</param>
    Private Sub AddTarget(ByVal row As Integer, ByVal column As Integer)
        If row >= 0 AndAlso column >= 0 _
            AndAlso row < EnemyGrid.Height _
            AndAlso column < EnemyGrid.Width _
            AndAlso EnemyGrid.Item(row, column) = TileView.Sea Then

            _Targets.Push(New Location(row, column))
        End If
    End Sub
End Class
