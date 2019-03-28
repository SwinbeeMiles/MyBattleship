''' <summary>
''' The SeaGrid is the grid upon which the ships are deployed.
''' </summary>
''' <remarks>
''' The grid is viewable via the ISeaGrid interface as a read only
''' grid. This can be used in conjuncture with the SeaGridAdapter to 
''' mask the position of the ships.
''' </remarks>
Public Class SeaGrid
    Implements ISeaGrid

    Private Const _WIDTH As Integer = 10
    Private Const _HEIGHT As Integer = 10

    Private _GameTiles(0 To Width - 1, 0 To Height - 1) As Tile
    Private _Ships As Dictionary(Of ShipName, Ship)
    Private _ShipsKilled As Integer = 0

    ''' <summary>
    ''' The sea grid has changed and should be redrawn.
    ''' </summary>
    Public Event Changed As EventHandler Implements ISeaGrid.Changed

    ''' <summary>
    ''' The width of the sea grid.
    ''' </summary>
    ''' <value>The width of the sea grid.</value>
    ''' <returns>The width of the sea grid.</returns>
    Public ReadOnly Property Width() As Integer Implements ISeaGrid.Width
        Get
            Return _WIDTH
        End Get
    End Property

    ''' <summary>
    ''' The height of the sea grid
    ''' </summary>
    ''' <value>The height of the sea grid</value>
    ''' <returns>The height of the sea grid</returns>
    Public ReadOnly Property Height() As Integer Implements ISeaGrid.Height
        Get
            Return _HEIGHT
        End Get
    End Property

    ''' <summary>
    ''' ShipsKilled returns the number of ships killed
    ''' </summary>
    Public ReadOnly Property ShipsKilled() As Integer
        Get
            Return _ShipsKilled
        End Get
    End Property

    ''' <summary>
    ''' Show the tile view
    ''' </summary>
    ''' <param name="x">x coordinate of the tile</param>
    ''' <param name="y">y coordiante of the tile</param>
    ''' <returns></returns>
    Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As TileView Implements ISeaGrid.Item
        Get
            Return _GameTiles(x, y).View
        End Get
    End Property

    ''' <summary>
    ''' AllDeployed checks if all the ships are deployed
    ''' </summary>
    Public ReadOnly Property AllDeployed() As Boolean
        Get
            For Each s As Ship In _Ships.Values
                If Not s.IsDeployed Then
                    Return False
                End If
            Next

            Return True
        End Get
    End Property

    ''' <summary>
    ''' SeaGrid constructor, a seagrid has a number of tiles stored in an array
    ''' </summary>
    Public Sub New(ByVal ships As Dictionary(Of ShipName, Ship))
        'fill array with empty Tiles
        Dim i as Integer
For i  = 0 To Width - 1
            For j As Integer = 0 To Height - 1
                _GameTiles(i, j) = New Tile(i, j, Nothing)
            Next
        Next

        _Ships = ships
    End Sub

    ''' <summary>
    ''' MoveShips allows for ships to be placed on the seagrid
    ''' </summary>
    ''' <param name="row">the row selected</param>
    ''' <param name="col">the column selected</param>
    ''' <param name="ship">the ship selected</param>
    ''' <param name="direction">the direction the ship is going</param>
    Public Sub MoveShip(ByVal row As Integer, ByVal col As Integer, ByVal ship As ShipName, ByVal direction As Direction)
        Dim newShip As Ship = _Ships(ship)
        newShip.Remove()
        AddShip(row, col, direction, newShip)
    End Sub

    ''' <summary>
    ''' AddShip add a ship to the SeaGrid
    ''' </summary>
    ''' <param name="row">row coordinate</param>
    ''' <param name="col">col coordinate</param>
    ''' <param name="direction">direction of ship</param>
    ''' <param name="newShip">the ship</param>
    Private Sub AddShip(ByVal row As Integer, ByVal col As Integer, ByVal direction As Direction, ByVal newShip As Ship)
        Try
            Dim size As Integer = newShip.Size
            Dim currentRow As Integer = row
            Dim currentCol As Integer = col
            Dim dRow, dCol As Integer

            If direction = direction.LeftRight Then
                dRow = 0
                dCol = 1
            Else
                dRow = 1
                dCol = 0
            End If

            'place ship's tiles in array and into ship object
            Dim i as Integer
For i  = 0 To size - 1
                If currentRow < 0 Or currentRow >= Width Or currentCol < 0 Or currentCol >= Height Then
                    Throw New InvalidOperationException("Ship can't fit on the board")
                End If

                _GameTiles(currentRow, currentCol).Ship = newShip

                currentCol += dCol
                currentRow += dRow
            Next

            newShip.Deployed(direction, row, col)
        Catch e As Exception
            newShip.Remove() 'if fails remove the ship
            Throw New ApplicationException(e.Message)

        Finally
            RaiseEvent Changed(Me, EventArgs.Empty)
        End Try
    End Sub

    ''' <summary>
    ''' HitTile hits a tile at a row/col, and whatever tile has been hit, a
    ''' result will be displayed.
    ''' </summary>
    ''' <param name="row">the row at which is being shot</param>
    ''' <param name="col">the cloumn at which is being shot</param>
    ''' <returns>An attackresult (hit, miss, sunk, shotalready)</returns>
    Public Function HitTile(ByVal row As Integer, ByVal col As Integer) As AttackResult Implements ISeaGrid.HitTile
        Try
            'tile is already hit
            If _GameTiles(row, col).Shot Then
                Return New AttackResult(ResultOfAttack.ShotAlready, "have already attacked [" & col & "," & row & "]!", row, col)
            End If

            _GameTiles(row, col).Shoot()

            'there is no ship on the tile
            If _GameTiles(row, col).Ship Is Nothing Then
                Return New AttackResult(ResultOfAttack.Miss, "missed", row, col)
            End If

            'all ship's tiles have been destroyed
            If _GameTiles(row, col).Ship.IsDestroyed Then
                _GameTiles(row, col).Shot = True
                _ShipsKilled += 1
                Return New AttackResult(ResultOfAttack.Destroyed, _GameTiles(row, col).Ship, "destroyed the enemy's", row, col)
            End If

            'else hit but not destroyed
            Return New AttackResult(ResultOfAttack.Hit, "hit something!", row, col)
        Finally
            RaiseEvent Changed(Me, EventArgs.Empty)
        End Try
    End Function
End Class
