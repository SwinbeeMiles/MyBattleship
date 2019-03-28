''' <summary>
''' Tile knows its location on the grid, if it is a ship and if it has been 
''' shot before
''' </summary>
Public Class Tile
    Private ReadOnly _RowValue As Integer        'the row value of the tile
    Private ReadOnly _ColumnValue As Integer     'the column value of the tile
    Private _Ship As Ship = Nothing     'the ship the tile belongs to
    Private _Shot As Boolean = False    'the tile has been shot at

    ''' <summary>
    ''' Has the tile been shot?
    ''' </summary>
    ''' <value>indicate if the tile has been shot</value>
    ''' <returns>true if the tile was shot</returns>
    Public Property Shot() As Boolean
        Get
            Return _Shot
        End Get
        Set(ByVal value As Boolean)
            _Shot = value
        End Set
    End Property

    ''' <summary>
    ''' The row of the tile in the grid
    ''' </summary>
    ''' <value>the row index of the tile in the grid</value>
    ''' <returns>the row index of the tile</returns>
    Public ReadOnly Property Row() As Integer
        Get
            Return _RowValue
        End Get
    End Property

    ''' <summary>
    ''' The column of the tile in the grid
    ''' </summary>
    ''' <value>the column of the tile in the grid</value>
    ''' <returns>the column of the tile in the grid</returns>
    Public ReadOnly Property Column() As Integer
        Get
            Return _ColumnValue
        End Get
    End Property

    ''' <summary>
    ''' Ship allows for a tile to check if there is ship and add a ship to a tile
    ''' </summary>
    Public Property Ship() As Ship
        Get
            Return _Ship
        End Get
        Set(ByVal value As Ship)
            If _Ship Is Nothing Then
                _Ship = value
                If value IsNot Nothing Then
                    _Ship.AddTile(Me)
                End If
            Else
                Throw New InvalidOperationException("There is already a ship at [" & Row & ", " & Column & "]")
            End If
        End Set
    End Property

    ''' <summary>
    ''' The tile constructor will know where it is on the grid, and is its a ship
    ''' </summary>
    ''' <param name="row">the row on the grid</param>
    ''' <param name="col">the col on the grid</param>
    ''' <param name="ship">what ship it is</param>
    Public Sub New(ByVal row As Integer, ByVal col As Integer, ByVal ship As Ship)
        _RowValue = row
        _ColumnValue = col
        _Ship = ship
    End Sub

    ''' <summary>
    ''' Clearship will remove the ship from the tile
    ''' </summary>
    Public Sub ClearShip()
        _Ship = Nothing
    End Sub

    ''' <summary>
    ''' View is able to tell the grid what the tile is
    ''' </summary>
    Public ReadOnly Property View() As TileView
        Get
            'if there is no ship in the tile
            If _Ship Is Nothing Then
                'and the tile has been hit
                If _Shot Then

                    Return TileView.Miss
                Else
                    'and the tile hasn't been hit
                    Return TileView.Sea
                End If
            Else
                'if there is a ship and it has been hit
                If (_Shot) Then
                    Return TileView.Hit
                Else
                    'if there is a ship and it hasn't been hit
                    Return TileView.Ship
                End If
            End If
        End Get
    End Property

    ''' <summary>
    ''' Shoot allows a tile to be shot at, and if the tile has been hit before
    ''' it will give an error
    ''' </summary>
    Friend Sub Shoot()
        If (False = Shot) Then
            Shot = True
            If _Ship IsNot Nothing Then
                _Ship.Hit()
            End If
        Else
            Throw New ApplicationException("You have already shot this square")
        End If
    End Sub
End Class
