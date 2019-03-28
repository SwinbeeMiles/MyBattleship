''' <summary>
''' This includes a number of utility methods for
''' drawing and interacting with the Mouse.
''' </summary>
Module UtilityFunctions
    Public Const FIELD_TOP As Integer = 122
    Public Const FIELD_LEFT As Integer = 349
    Public Const FIELD_WIDTH As Integer = 418
    Public Const FIELD_HEIGHT As Integer = 418

    Public Const MESSAGE_TOP As Integer = 548

    Public Const CELL_WIDTH As Integer = 40
    Public Const CELL_HEIGHT As Integer = 40
    Public Const CELL_GAP As Integer = 2

    Public Const SHIP_GAP As Integer = 3

    Private ReadOnly SMALL_SEA As Color = SwinGame.RGBAColor(6, 60, 94, 255)
    Private ReadOnly SMALL_SHIP As Color = Color.Gray
    Private ReadOnly SMALL_MISS As Color = SwinGame.RGBAColor(1, 147, 220, 255)
    Private ReadOnly SMALL_HIT As Color = SwinGame.RGBAColor(169, 24, 37, 255)

    Private ReadOnly LARGE_SEA As Color = SwinGame.RGBAColor(6, 60, 94, 255)
    Private ReadOnly LARGE_SHIP As Color = Color.Gray
    Private ReadOnly LARGE_MISS As Color = SwinGame.RGBAColor(1, 147, 220, 255)
    Private ReadOnly LARGE_HIT As Color = SwinGame.RGBAColor(252, 2, 3, 255)

    Private ReadOnly OUTLINE_COLOR As Color = SwinGame.RGBAColor(5, 55, 88, 255)
    Private ReadOnly SHIP_FILL_COLOR As Color = Color.Gray
    Private ReadOnly SHIP_OUTLINE_COLOR As Color = Color.White
    Private ReadOnly MESSAGE_COLOR As Color = SwinGame.RGBAColor(2, 167, 252, 255)

    Public Const ANIMATION_CELLS As Integer = 7
    Public Const FRAMES_PER_CELL As Integer = 8

    ''' <summary>
    ''' Determines if the mouse is in a given rectangle.
    ''' </summary>
    ''' <param name="x">the x location to check</param>
    ''' <param name="y">the y location to check</param>
    ''' <param name="w">the width to check</param>
    ''' <param name="h">the height to check</param>
    ''' <returns>true if the mouse is in the area checked</returns>
    Public Function IsMouseInRectangle(ByVal x As Integer, ByVal y As Integer, ByVal w As Integer, ByVal h As Integer) As Boolean
        Dim mouse As Point2D
        Dim result As Boolean = False

        mouse = SwinGame.MousePosition()

        'if the mouse is inline with the button horizontally
        If mouse.X >= x And mouse.X <= x + w Then
            'Check vertical position
            If mouse.Y >= y And mouse.Y <= y + h Then
                result = True
            End If
        End If

        Return result
    End Function

    ''' <summary>
    ''' Draws a large field using the grid and the indicated player's ships.
    ''' </summary>
    ''' <param name="grid">the grid to draw</param>
    ''' <param name="thePlayer">the players ships to show</param>
    ''' <param name="showShips">indicates if the ships should be shown</param>
    Public Sub DrawField(ByVal grid As ISeaGrid, ByVal thePlayer As Player, ByVal showShips As Boolean)
        DrawCustomField(grid, thePlayer, False, showShips, FIELD_LEFT, FIELD_TOP, FIELD_WIDTH, FIELD_HEIGHT, CELL_WIDTH, CELL_HEIGHT, CELL_GAP)
    End Sub

    ''' <summary>
    ''' Draws a small field, showing the attacks made and the locations of the player's ships
    ''' </summary>
    ''' <param name="grid">the grid to show</param>
    ''' <param name="thePlayer">the player to show the ships of</param>
    Public Sub DrawSmallField(ByVal grid As ISeaGrid, ByVal thePlayer As Player)
        Const SMALL_FIELD_LEFT As Integer = 39, SMALL_FIELD_TOP As Integer = 373
        Const SMALL_FIELD_WIDTH As Integer = 166, SMALL_FIELD_HEIGHT As Integer = 166
        Const SMALL_FIELD_CELL_WIDTH As Integer = 13, SMALL_FIELD_CELL_HEIGHT As Integer = 13
        Const SMALL_FIELD_CELL_GAP As Integer = 4

        DrawCustomField(grid, thePlayer, True, True, SMALL_FIELD_LEFT, SMALL_FIELD_TOP, SMALL_FIELD_WIDTH, SMALL_FIELD_HEIGHT, SMALL_FIELD_CELL_WIDTH, SMALL_FIELD_CELL_HEIGHT, SMALL_FIELD_CELL_GAP)
    End Sub

    ''' <summary>
    ''' Draws the player's grid and ships.
    ''' </summary>
    ''' <param name="grid">the grid to show</param>
    ''' <param name="thePlayer">the player to show the ships of</param>
    ''' <param name="small">true if the small grid is shown</param>
    ''' <param name="showShips">true if ships are to be shown</param>
    ''' <param name="left">the left side of the grid</param>
    ''' <param name="top">the top of the grid</param>
    ''' <param name="width">the width of the grid</param>
    ''' <param name="height">the height of the grid</param>
    ''' <param name="cellWidth">the width of each cell</param>
    ''' <param name="cellHeight">the height of each cell</param>
    ''' <param name="cellGap">the gap between the cells</param>
    Private Sub DrawCustomField(ByVal grid As ISeaGrid, ByVal thePlayer As Player, ByVal small As Boolean, ByVal showShips As Boolean, ByVal left As Integer, ByVal top As Integer, ByVal width As Integer, ByVal height As Integer, ByVal cellWidth As Integer, ByVal cellHeight As Integer, ByVal cellGap As Integer)
        'SwinGame.FillRectangle(Color.Blue, left, top, width, height)

        Dim rowTop As Integer
        Dim colLeft As Integer

        'Draw the grid
        For row As Integer = 0 To 9
            rowTop = top + (cellGap + cellHeight) * row

            For col As Integer = 0 To 9
                colLeft = left + (cellGap + cellWidth) * col

                Dim fillColor As Color
                Dim draw As Boolean

                draw = True

                Select Case grid.Item(row, col)
                    Case TileView.Ship
                        draw = False
                        'If small Then fillColor = _SMALL_SHIP Else fillColor = _LARGE_SHIP
                    Case TileView.Miss
                        If small Then fillColor = SMALL_MISS Else fillColor = LARGE_MISS
                    Case TileView.Hit
                        If small Then fillColor = SMALL_HIT Else fillColor = LARGE_HIT
                    Case TileView.Sea, TileView.Ship
                        If small Then fillColor = SMALL_SEA Else draw = False
                End Select

                If draw Then
                    SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight)
                    If Not small Then
                        SwinGame.DrawRectangle(OUTLINE_COLOR, colLeft, rowTop, cellWidth, cellHeight)
                    End If
                End If
            Next
        Next

        If Not showShips Then
            Exit Sub
        End If

        Dim shipHeight, shipWidth As Integer
        Dim shipName As String

        'Draw the ships
        For Each s As Ship In thePlayer
            If s Is Nothing OrElse Not s.IsDeployed Then Continue For
            rowTop = top + (cellGap + cellHeight) * s.Row + SHIP_GAP
            colLeft = left + (cellGap + cellWidth) * s.Column + SHIP_GAP

            If s.Direction = Direction.LeftRight Then
                shipName = "ShipLR" & s.Size
                shipHeight = cellHeight - (SHIP_GAP * 2)
                shipWidth = (cellWidth + cellGap) * s.Size - (SHIP_GAP * 2) - cellGap
            Else
                'Up down
                shipName = "ShipUD" & s.Size
                shipHeight = (cellHeight + cellGap) * s.Size - (SHIP_GAP * 2) - cellGap
                shipWidth = cellWidth - (SHIP_GAP * 2)
            End If

            If Not small Then
                SwinGame.DrawBitmap(GameImage(shipName), colLeft, rowTop)
            Else
                SwinGame.FillRectangle(SHIP_FILL_COLOR, colLeft, rowTop, shipWidth, shipHeight)
                SwinGame.DrawRectangle(SHIP_OUTLINE_COLOR, colLeft, rowTop, shipWidth, shipHeight)
            End If
        Next
    End Sub

    Private _message As String

    ''' <summary>
    ''' The message to display
    ''' </summary>
    ''' <value>The message to display</value>
    ''' <returns>The message to display</returns>
    Public Property Message() As String
        Get
            Return _message
        End Get
        Set(ByVal value As String)
            _message = value
        End Set
    End Property

    ''' <summary>
    ''' Draws the message to the screen
    ''' </summary>
    Public Sub DrawMessage()
        SwinGame.DrawText(Message, MESSAGE_COLOR, GameFont("Courier"), FIELD_LEFT, MESSAGE_TOP)
    End Sub

    ''' <summary>
    ''' Draws the background for the current state of the game
    ''' </summary>
    Public Sub DrawBackground()

        Select Case CurrentState
            Case GameState.ViewingMainMenu, GameState.ViewingGameMenu, GameState.AlteringSettings, GameState.ViewingHighScores
                SwinGame.DrawBitmap(GameImage("Menu"), 0, 0)
            Case GameState.Discovering, GameState.EndingGame
                SwinGame.DrawBitmap(GameImage("Discovery"), 0, 0)
            Case GameState.Deploying
                SwinGame.DrawBitmap(GameImage("Deploy"), 0, 0)
            Case Else
                SwinGame.ClearScreen()
        End Select

        SwinGame.DrawFramerate(675, 585, GameFont("CourierSmall"))
    End Sub

    Public Sub AddExplosion(ByVal row As Integer, ByVal col As Integer)
        AddAnimation(row, col, "Splash")
    End Sub

    Public Sub AddSplash(ByVal row As Integer, ByVal col As Integer)
        AddAnimation(row, col, "Splash")
    End Sub

    Private _Animations As New List(Of Sprite)()

    Private Sub AddAnimation(ByVal row As Integer, ByVal col As Integer, ByVal image As String)
        Dim s As Sprite
        Dim imgObj as Bitmap

        imgObj = GameImage(image)
        imgObj.SetCellDetails(40, 40, 3, 3, 7)

        Dim animation as AnimationScript
        animation = SwinGame.LoadAnimationScript("splash.txt")

        s = SwinGame.CreateSprite(imgObj, animation)
        s.X = FIELD_LEFT + col * (CELL_WIDTH + CELL_GAP)
        s.Y = FIELD_TOP + row * (CELL_HEIGHT + CELL_GAP)

        s.StartAnimation("splash")
        _Animations.Add(s)
    End Sub

    Public Sub UpdateAnimations()
        Dim ended As New List(Of Sprite)()
        For Each s As Sprite In _Animations
            SwinGame.UpdateSprite(s)
            If s.animationHasEnded Then
                ended.Add(s)
            End If
        Next

        For Each s As Sprite In ended
            _Animations.Remove(s)
            SwinGame.FreeSprite(s)
        Next
    End Sub

    Public Sub DrawAnimations()
        For Each s As Sprite In _Animations
            SwinGame.DrawSprite(s)
        Next
    End Sub

    Public Sub DrawAnimationSequence()
        Dim i as Integer
For i  = 1 To ANIMATION_CELLS * FRAMES_PER_CELL
            UpdateAnimations()
            DrawScreen()
        Next
    End Sub
End Module
