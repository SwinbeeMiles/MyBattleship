''' <summary>
''' A Ship has all the details about itself. For example the shipname,
''' size, number of hits taken and the location. Its able to add tiles,
''' remove, hits taken and if its deployed and destroyed.
''' </summary>
''' <remarks>
''' Deployment information is supplied to allow ships to be drawn.
''' </remarks>
Public Class Ship
    Private _shipName As ShipName
    Private _sizeOfShip As Integer
    Private _hitsTaken As Integer = 0
    Private _tiles As List(Of Tile)
    Private _row As Integer
    Private _col As Integer
    Private _direction As Direction

    ''' <summary>
    ''' The type of ship
    ''' </summary>
    ''' <value>The type of ship</value>
    ''' <returns>The type of ship</returns>
    Public ReadOnly Property Name() As String
        Get
            If _shipName = ShipName.AircraftCarrier Then
                Return "Aircraft Carrier"
            End If

            Return _shipName.ToString()
        End Get
    End Property

    ''' <summary>
    ''' The number of cells that this ship occupies.
    ''' </summary>
    ''' <value>The number of hits the ship can take</value>
    ''' <returns>The number of hits the ship can take</returns>
    Public ReadOnly Property Size() As Integer
        Get
            Return _sizeOfShip
        End Get
    End Property

    ''' <summary>
    ''' The number of hits that the ship has taken.
    ''' </summary>
    ''' <value>The number of hits the ship has taken.</value>
    ''' <returns>The number of hits the ship has taken</returns>
    ''' <remarks>When this equals Size the ship is sunk</remarks>
    Public ReadOnly Property Hits() As Integer
        Get
            Return _hitsTaken
        End Get
    End Property

    ''' <summary>
    ''' The row location of the ship
    ''' </summary>
    ''' <value>The topmost location of the ship</value>
    ''' <returns>the row of the ship</returns>
    Public ReadOnly Property Row() As Integer
        Get
            Return _row
        End Get
    End Property

    Public ReadOnly Property Column() As Integer
        Get
            Return _col
        End Get
    End Property

    Public ReadOnly Property Direction() As Direction
        Get
            Return _direction
        End Get
    End Property

    Public Sub New(ByVal ship As ShipName)
        _shipName = ship
        _tiles = New List(Of Tile)()

        'gets the ship size from the enumarator
        _sizeOfShip = _shipName
    End Sub

    ''' <summary>
    ''' Add tile adds the ship tile
    ''' </summary>
    ''' <param name="tile">one of the tiles the ship is on</param>
    Public Sub AddTile(ByVal tile As Tile)
        _tiles.Add(tile)
    End Sub

    ''' <summary>
    ''' Remove clears the tile back to a sea tile
    ''' </summary>
    Public Sub Remove()
        For Each tile As Tile In _tiles
            tile.ClearShip()
        Next
        _tiles.Clear()
    End Sub

    Public Sub Hit()
        _hitsTaken = _hitsTaken + 1
    End Sub

    ''' <summary>
    ''' IsDeployed returns if the ships is deployed, if its deplyed it has more than
    ''' 0 tiles
    ''' </summary>
    Public ReadOnly Property IsDeployed() As Boolean
        Get
            Return _tiles.Count > 0
        End Get
    End Property

    Public ReadOnly Property IsDestroyed() As Boolean
        Get
            Return Hits = Size
        End Get
    End Property

    ''' <summary>
    ''' Record that the ship is now deployed.
    ''' </summary>
    ''' <param name="direction"></param>
    ''' <param name="row"></param>
    ''' <param name="col"></param>
    Friend Sub Deployed(ByVal direction As Direction, ByVal row As Integer, ByVal col As Integer)
        _row = row
        _col = col
        _direction = direction
    End Sub
End Class
