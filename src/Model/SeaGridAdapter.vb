''' <summary>
''' The SeaGridAdapter allows for the change in a sea grid view. Whenever a ship is
''' presented it changes the view into a sea tile instead of a ship tile.
''' </summary>
Public Class SeaGridAdapter
    Implements ISeaGrid

    Private _MyGrid As SeaGrid

    ''' <summary>
    ''' Create the SeaGridAdapter, with the grid, and it will allow it to be changed
    ''' </summary>
    ''' <param name="grid">the grid that needs to be adapted</param>
    Public Sub New(ByVal grid As SeaGrid)
        _MyGrid = grid
        AddHandler _MyGrid.Changed, New EventHandler(AddressOf MyGrid_Changed)
    End Sub

    ''' <summary>
    ''' MyGrid_Changed causes the grid to be redrawn by raising a changed event
    ''' </summary>
    ''' <param name="sender">the object that caused the change</param>
    ''' <param name="e">what needs to be redrawn</param>
    Private Sub MyGrid_Changed(ByVal sender As Object, ByVal e As EventArgs)
        RaiseEvent Changed(Me, e)
    End Sub

#Region "ISeaGrid Members"

    ''' <summary>
    ''' Changes the discovery grid. Where there is a ship we will sea water
    ''' </summary>
    ''' <param name="x">tile x coordinate</param>
    ''' <param name="y">tile y coordinate</param>
    ''' <returns>a tile, either what it actually is, or if it was a ship then return a sea tile</returns>
    Public ReadOnly Property Item(ByVal x As Integer, ByVal y As Integer) As TileView Implements ISeaGrid.Item
        Get
            Dim result As TileView = _MyGrid.Item(x, y)

            If result = TileView.Ship Then
                Return TileView.Sea
            Else
                Return result
            End If
        End Get
    End Property

    ''' <summary>
    ''' Indicates that the grid has been changed
    ''' </summary>
    Public Event Changed As EventHandler Implements ISeaGrid.Changed

    ''' <summary>
    ''' Get the width of a tile
    ''' </summary>
    Public ReadOnly Property Width() As Integer Implements ISeaGrid.Width
        Get
            Return _MyGrid.Width
        End Get
    End Property

    ''' <summary>
    ''' Get the height of the tile
    ''' </summary>
    Public ReadOnly Property Height() As Integer Implements ISeaGrid.Height
        Get
            Return _MyGrid.Height
        End Get
    End Property

    ''' <summary>
    ''' HitTile calls oppon _MyGrid to hit a tile at the row, col
    ''' </summary>
    ''' <param name="row">the row its hitting at</param>
    ''' <param name="col">the column its hitting at</param>
    ''' <returns>The result from hitting that tile</returns>
    Public Function HitTile(ByVal row As Integer, ByVal col As Integer) As AttackResult Implements ISeaGrid.HitTile
        Return _MyGrid.HitTile(row, col)
    End Function
#End Region

End Class
