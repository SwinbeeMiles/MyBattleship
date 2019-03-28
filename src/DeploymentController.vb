Imports SwinGameSDK

''' <summary>
''' The DeploymentController controls the players actions
''' during the deployment phase.
''' </summary>
Module DeploymentController
    Private Const SHIPS_TOP As Integer = 98
    Private Const SHIPS_LEFT As Integer = 20
    Private Const SHIPS_HEIGHT As Integer = 90
    Private Const SHIPS_WIDTH As Integer = 300

    Private Const TOP_BUTTONS_TOP As Integer = 72
    Private Const TOP_BUTTONS_HEIGHT As Integer = 46

    Private Const PLAY_BUTTON_LEFT As Integer = 693
    Private Const PLAY_BUTTON_WIDTH As Integer = 80    

    Private Const UP_DOWN_BUTTON_LEFT As Integer = 410
    Private Const LEFT_RIGHT_BUTTON_LEFT As Integer = 350

    Private Const RANDOM_BUTTON_LEFT As Integer = 547
    Private Const RANDOM_BUTTON_WIDTH As Integer = 51

    Private Const DIR_BUTTONS_WIDTH As Integer = 47

    Private Const TEXT_OFFSET As Integer = 5

    Private _currentDirection As Direction = Direction.UpDown
    Private _selectedShip As ShipName = ShipName.Tug

    ''' <summary>
    ''' Handles user input for the Deployment phase of the game.
    ''' </summary>
    ''' <remarks>
    ''' Involves selecting the ships, deloying ships, changing the direction
    ''' of the ships to add, randomising deployment, end then ending
    ''' deployment
    ''' </remarks>
    Public Sub HandleDeploymentInput()
        If SwinGame.KeyTyped(KeyCode.VK_ESCAPE) Then
            AddNewState(GameState.ViewingGameMenu)
        End If

        If SwinGame.KeyTyped(KeyCode.VK_UP) Or SwinGame.KeyTyped(KeyCode.VK_DOWN) Then
            _currentDirection = Direction.UpDown
        End If
        If SwinGame.KeyTyped(KeyCode.VK_LEFT) Or SwinGame.KeyTyped(KeyCode.VK_RIGHT) Then
            _currentDirection = Direction.LeftRight
        End If

        If SwinGame.KeyTyped(KeyCode.VK_R) Then
            HumanPlayer.RandomizeDeployment()
        End If

        If SwinGame.MouseClicked(MouseButton.LeftButton) Then
            Dim selected As ShipName
            selected = GetShipMouseIsOver()
            If selected <> ShipName.None Then
                _selectedShip = selected
            Else
                DoDeployClick()
            End If

            If HumanPlayer.ReadyToDeploy And IsMouseInRectangle(PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP, PLAY_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT) Then
                EndDeployment()
            ElseIf IsMouseInRectangle(UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT) Then
                _currentDirection = Direction.LeftRight
            ElseIf IsMouseInRectangle(LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP, DIR_BUTTONS_WIDTH, TOP_BUTTONS_HEIGHT) Then
                _currentDirection = Direction.LeftRight
            ElseIf IsMouseInRectangle(RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP, RANDOM_BUTTON_WIDTH, TOP_BUTTONS_HEIGHT) Then
                HumanPlayer.RandomizeDeployment()
            End If
        End If
    End Sub

    ''' <summary>
    ''' The user has clicked somewhere on the screen, check if its is a deployment and deploy
    ''' the current ship if that is the case.
    ''' </summary>
    ''' <remarks>
    ''' If the click is in the grid it deploys to the selected location
    ''' with the indicated direction
    ''' </remarks>
    Private Sub DoDeployClick()
        Dim mouse As Point2D

        mouse = SwinGame.MousePosition()

        'Calculate the row/col clicked
        Dim row, col As Integer
        row = Convert.ToInt32(Math.Floor((mouse.Y) / (CELL_HEIGHT + CELL_GAP)))
        col = Convert.ToInt32(Math.Floor((mouse.X - FIELD_LEFT) / (CELL_WIDTH + CELL_GAP)))

        If row >= 0 And row < HumanPlayer.PlayerGrid.Height Then
            If col >= 0 And col < HumanPlayer.PlayerGrid.Width Then
                'if in the area try to deploy
                Try
                    HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection)
                Catch ex As Exception
                    Audio.PlaySoundEffect(GameSound("Error"))
                    Message = ex.Message
                End Try
            End If
        End If
    End Sub

    ''' <summary>
    ''' Draws the deployment screen showing the field and the ships
    ''' that the player can deploy.
    ''' </summary>
    Public Sub DrawDeployment()
        DrawField(HumanPlayer.PlayerGrid, HumanPlayer, True)

        'Draw the Left/Right and Up/Down buttons
        If _currentDirection = Direction.LeftRight Then
            SwinGame.DrawBitmap(GameImage("LeftRightButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
            'SwinGame.DrawText("U/D", Color.Gray, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
            'SwinGame.DrawText("L/R", Color.White, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
        Else
            SwinGame.DrawBitmap(GameImage("UpDownButton"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
            'SwinGame.DrawText("U/D", Color.White, GameFont("Menu"), UP_DOWN_BUTTON_LEFT, TOP_BUTTONS_TOP)
            'SwinGame.DrawText("L/R", Color.Gray, GameFont("Menu"), LEFT_RIGHT_BUTTON_LEFT, TOP_BUTTONS_TOP)
        End If

        'DrawShips
        For Each sn As ShipName In [Enum].GetValues(GetType(ShipName))
            Dim i As Integer
            i = Int(sn) - 1
            If i >= 0 Then
                If sn = _selectedShip Then
                    SwinGame.DrawBitmap(GameImage("SelectedShip"), SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT)
                    '    SwinGame.FillRectangle(Color.LightBlue, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
                    'Else
                    '    SwinGame.FillRectangle(Color.Gray, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
                End If

                'SwinGame.DrawRectangle(Color.Black, SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT)
                'SwinGame.DrawText(sn.ToString(), Color.Black, GameFont("Courier"), SHIPS_LEFT + TEXT_OFFSET, SHIPS_TOP + i * SHIPS_HEIGHT)

            End If
        Next

        If HumanPlayer.ReadyToDeploy Then
            SwinGame.DrawBitmap(GameImage("PlayButton"), PLAY_BUTTON_LEFT, TOP_BUTTONS_TOP)
            'SwinGame.FillRectangle(Color.LightBlue, PLAY_BUTTON_LEFT, PLAY_BUTTON_TOP, PLAY_BUTTON_WIDTH, PLAY_BUTTON_HEIGHT)
            'SwinGame.DrawText("PLAY", Color.Black, GameFont("Courier"), PLAY_BUTTON_LEFT + TEXT_OFFSET, PLAY_BUTTON_TOP)
        End If

        SwinGame.DrawBitmap(GameImage("RandomButton"), RANDOM_BUTTON_LEFT, TOP_BUTTONS_TOP)

        DrawMessage()
    End Sub

    ''' <summary>
    ''' Gets the ship that the mouse is currently over in the selection panel.
    ''' </summary>
    ''' <returns>The ship selected or none</returns>
    Private Function GetShipMouseIsOver() As ShipName
        For Each sn As ShipName In [Enum].GetValues(GetType(ShipName))
            Dim i As Integer
            i = Int(sn) - 1

            If IsMouseInRectangle(SHIPS_LEFT, SHIPS_TOP + i * SHIPS_HEIGHT, SHIPS_WIDTH, SHIPS_HEIGHT) Then
                Return sn
            End If
        Next

        Return ShipName.None
    End Function
End Module
