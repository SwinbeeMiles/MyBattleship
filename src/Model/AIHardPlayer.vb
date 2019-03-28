''' <summary>
''' AIHardPlayer is a type of player. This AI will know directions of ships
''' when it has found 2 ship tiles and will try to destroy that ship. If that ship
''' is not destroyed it will shoot the other way. Ship still not destroyed, then
''' the AI knows it has hit multiple ships. Then will try to destoy all around tiles
''' that have been hit.
''' </summary>
Public Class AIHardPlayer : Inherits AIPlayer

    ''' <summary>
    ''' Target allows the AI to know more things, for example the source of a
    ''' shot target
    ''' </summary>
    Protected Class Target
        Private ReadOnly _ShotAt As Location
        Private ReadOnly _Source As Location

        ''' <summary>
        ''' The target shot at
        ''' </summary>
        ''' <value>The target shot at</value>
        ''' <returns>The target shot at</returns>
        Public ReadOnly Property ShotAt() As Location
            Get
                Return _ShotAt
            End Get
        End Property

        ''' <summary>
        ''' The source that added this location as a target.
        ''' </summary>
        ''' <value>The source that added this location as a target.</value>
        ''' <returns>The source that added this location as a target.</returns>
        Public ReadOnly Property Source() As Location
            Get
                Return _Source
            End Get
        End Property

        Friend Sub New(ByVal shootat As Location, ByVal source As Location)
            _ShotAt = shootat
            _Source = source
        End Sub

        ''' <summary>
        ''' If source shot and shootat shot are on the same row then 
        ''' give a boolean true
        ''' </summary>
        Public ReadOnly Property SameRow() As Boolean
            Get
                Return _ShotAt.Row = _Source.Row
            End Get
        End Property

        ''' <summary>
        ''' If source shot and shootat shot are on the same column then 
        ''' give a boolean true 
        ''' </summary>
        Public ReadOnly Property SameColumn() As Boolean
            Get
                Return _ShotAt.Column = _Source.Column
            End Get
        End Property
    End Class

    ''' <summary>
    ''' Private enumarator for AI states. currently there are two states,
    ''' the AI can be searching for a ship, or if it has found a ship it will
    ''' target the same ship
    ''' </summary>
    Private Enum AIStates
        ''' <summary>
        ''' The AI is searching for its next target
        ''' </summary>
        Searching

        ''' <summary>
        ''' The AI is trying to target a ship
        ''' </summary>
        TargetingShip

        ''' <summary>
        ''' The AI is locked onto a ship
        ''' </summary>
        HittingShip
    End Enum

    Private _CurrentState As AIStates = AIStates.Searching
    Private _Targets As New Stack(Of Target)()
    Private _LastHit As New List(Of Target)()
    Private _CurrentTarget As Target

    Public Sub New(ByVal game As BattleShipsGame)
        MyBase.New(game)
    End Sub

    ''' <summary>
    ''' GenerateCoords will call upon the right methods to generate the appropriate shooting
    ''' coordinates
    ''' </summary>
    ''' <param name="row">the row that will be shot at</param>
    ''' <param name="column">the column that will be shot at</param>
    Protected Overrides Sub GenerateCoords(ByRef row As Integer, ByRef column As Integer)
        Do
            _CurrentTarget = Nothing

            'check which state the AI is in and uppon that choose which coordinate generation
            'method will be used.
            Select Case _CurrentState
                Case AIStates.Searching
                    SearchCoords(row, column)
                Case AIStates.TargetingShip, AIStates.HittingShip
                    TargetCoords(row, column)
                Case Else
                    Throw New ApplicationException("AI has gone in an invalid state")
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
        Dim t As Target
        t = _Targets.Pop()

        row = t.ShotAt.Row
        column = t.ShotAt.Column
        _CurrentTarget = t
    End Sub

    ''' <summary>
    ''' SearchCoords will randomly generate shots within the grid as long as its not hit that tile already
    ''' </summary>
    ''' <param name="row">the generated row</param>
    ''' <param name="column">the generated column</param>
    Private Sub SearchCoords(ByRef row As Integer, ByRef column As Integer)
        row = _Random.Next(0, EnemyGrid.Height)
        column = _Random.Next(0, EnemyGrid.Width)
        _CurrentTarget = New Target(New Location(row, column), Nothing)
    End Sub

    ''' <summary>
    ''' ProcessShot is able to process each shot that is made and call the right methods belonging
    ''' to that shot. For example, if its a miss = do nothing, if it's a hit = process that hit location
    ''' </summary>
    ''' <param name="row">the row that was shot at</param>
    ''' <param name="col">the column that was shot at</param>
    ''' <param name="result">the result from that hit</param>
    Protected Overrides Sub ProcessShot(ByVal row As Integer, ByVal col As Integer, ByVal result As AttackResult)
        Select Case result.Value
            Case ResultOfAttack.Miss
                _CurrentTarget = Nothing
            Case ResultOfAttack.Hit
                ProcessHit(row, col)
            Case ResultOfAttack.Destroyed
                ProcessDestroy(row, col, result.Ship)
            Case ResultOfAttack.ShotAlready
                Throw New ApplicationException("Error in AI")
        End Select

        If _Targets.Count = 0 Then _CurrentState = AIStates.Searching
    End Sub

    ''' <summary>
    ''' ProcessDetroy is able to process the destroyed ships targets and remove _LastHit targets.
    ''' It will also call RemoveShotsAround to remove targets that it was going to shoot at
    ''' </summary>
    ''' <param name="row">the row that was shot at and destroyed</param>
    ''' <param name="col">the row that was shot at and destroyed</param>
    ''' <param name="ship">the row that was shot at and destroyed</param>
    Private Sub ProcessDestroy(ByVal row As Integer, ByVal col As Integer, ByVal ship As Ship)
        Dim foundOriginal As Boolean
        Dim source As Location
        Dim current As Target
        current = _CurrentTarget

        foundOriginal = False

        'i = 1, as we dont have targets from the current hit...
        Dim i as Integer
        For i = 1 To ship.Hits - 1

            If Not foundOriginal Then
                source = current.Source
                'Source is nnothing if the ship was originally hit in
                ' the middle. This then searched forward, rather than
                ' backward through the list of targets
                If source Is Nothing Then
                    source = current.ShotAt
                    foundOriginal = True
                End If
            Else
                source = current.ShotAt
            End If

            'find the source in _LastHit
            For Each t As Target In _LastHit
                If (Not foundOriginal AndAlso t.ShotAt = source) _
                   OrElse (foundOriginal And t.Source = source) Then
                    current = t
                    _LastHit.Remove(t)
                    Exit For
                End If
            Next

            RemoveShotsAround(current.ShotAt)
        Next
    End Sub

    ''' <summary>
    ''' RemoveShotsAround will remove targets that belong to the destroyed ship by checking if 
    ''' the source of the targets belong to the destroyed ship. If they don't put them on a new stack.
    ''' Then clear the targets stack and move all the targets that still need to be shot at back 
    ''' onto the targets stack
    ''' </summary>
    ''' <param name="toRemove"></param>
    Private Sub RemoveShotsAround(ByVal toRemove As Location)
        Dim newStack As New Stack(Of Target)()  'create a new stack

        'check all targets in the _Targets stack
        For Each t As Target In _Targets

            'if the source of the target does not belong to the destroyed ship put them on the newStack
            If t.Source IsNot toRemove Then newStack.Push(t)
        Next

        _Targets.Clear()   'clear the _Targets stack

        'for all the targets in the newStack, move them back onto the _Targets stack
        For Each t As Target In newStack
            _Targets.Push(t)
        Next

        'if the _Targets stack is 0 then change the AI's state back to searching
        If _Targets.Count = 0 Then _CurrentState = AIStates.Searching
    End Sub

    ''' <summary>
    ''' ProcessHit gets the last hit location coordinates and will ask AddTarget to
    ''' create targets around that location by calling the method four times each time with
    ''' a new location around the last hit location.
    ''' It will then set the state of the AI and if it's not Searching or targetingShip then 
    ''' start ReOrderTargets.
    ''' </summary>
    ''' <param name="row"></param>
    ''' <param name="col"></param>
    Private Sub ProcessHit(ByVal row As Integer, ByVal col As Integer)
        _LastHit.Add(_CurrentTarget)

        'Uses _CurrentTarget as the source
        AddTarget(row - 1, col)
        AddTarget(row, col - 1)
        AddTarget(row + 1, col)
        AddTarget(row, col + 1)

        If _CurrentState = AIStates.Searching Then
            _CurrentState = AIStates.TargetingShip
        Else
            'either targetting or hitting... both are the same here
            _CurrentState = AIStates.HittingShip

            ReOrderTargets()
        End If
    End Sub

    ''' <summary>
    ''' ReOrderTargets will optimise the targeting by re-orderin the stack that the targets are in.
    ''' By putting the most important targets at the top they are the ones that will be shot at first.
    ''' </summary>
    Private Sub ReOrderTargets()

        'if the ship is lying on the same row, call MoveToTopOfStack to optimise on the row
        If _CurrentTarget.SameRow Then
            MoveToTopOfStack(_CurrentTarget.ShotAt.Row, -1)
        ElseIf _CurrentTarget.SameColumn Then
            'else if the ship is lying on the same column, call MoveToTopOfStack to optimise on the column
            MoveToTopOfStack(-1, _CurrentTarget.ShotAt.Column)
        End If
    End Sub

    ''' <summary>
    ''' MoveToTopOfStack will re-order the stack by checkin the coordinates of each target
    ''' If they have the right column or row values it will be moved to the _Match stack else 
    ''' put it on the _NoMatch stack. Then move all the targets from the _NoMatch stack back on the 
    ''' _Targets stack, these will be at the bottom making them less important. The move all the
    ''' targets from the _Match stack on the _Targets stack, these will be on the top and will there
    ''' for be shot at first
    ''' </summary>
    ''' <param name="row">the row of the optimisation</param>
    ''' <param name="column">the column of the optimisation</param>
    Private Sub MoveToTopOfStack(ByVal row As Integer, ByVal column As Integer)
        Dim _NoMatch As New Stack(Of Target)()
        Dim _Match As New Stack(Of Target)()

        Dim current As Target

        While _Targets.Count > 0
            current = _Targets.Pop()
            If current.ShotAt.Row = row OrElse current.ShotAt.Column = column Then
                _Match.Push(current)
            Else
                _NoMatch.Push(current)
            End If
        End While

        For Each t As Target In _NoMatch
            _Targets.Push(t)
        Next
        For Each t As Target In _Match
            _Targets.Push(t)
        Next
    End Sub

    ''' <summary>
    ''' AddTarget will add the targets it will shoot onto a stack
    ''' </summary>
    ''' <param name="row">the row of the targets location</param>
    ''' <param name="column">the column of the targets location</param>
    Private Sub AddTarget(ByVal row As Integer, ByVal column As Integer)

        If (row >= 0 AndAlso column >= 0 _
            AndAlso row < EnemyGrid.Height _
            AndAlso column < EnemyGrid.Width _
            AndAlso EnemyGrid.Item(row, column) = TileView.Sea) Then

            _Targets.Push(New Target(New Location(row, column), _CurrentTarget.ShotAt))
        End If
    End Sub

End Class
