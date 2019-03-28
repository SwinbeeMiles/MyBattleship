''' <summary>
''' The ISeaGrid defines the read only interface of a Grid. This
''' allows each player to see and attack their opponents grid.
''' </summary>
Public Interface ISeaGrid

    ReadOnly Property Width() As Integer

    ReadOnly Property Height() As Integer

    ''' <summary>
    ''' Indicates that the grid has changed.
    ''' </summary>
    Event Changed As EventHandler

    ''' <summary>
    ''' Provides access to the given row/column
    ''' </summary>
    ''' <param name="row">the row to access</param>
    ''' <param name="column">the column to access</param>
    ''' <value>what the player can see at that location</value>
    ''' <returns>what the player can see at that location</returns>
    ReadOnly Property Item(ByVal row As Integer, ByVal column As Integer) As TileView

    ''' <summary>
    ''' Mark the indicated tile as shot.
    ''' </summary>
    ''' <param name="row">the row of the tile</param>
    ''' <param name="col">the column of the tile</param>
    ''' <returns>the result of the attack</returns>
    Function HitTile(ByVal row As Integer, ByVal col As Integer) As AttackResult
End Interface
