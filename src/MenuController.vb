Imports SwinGameSDK

''' <summary>
''' The menu controller handles the drawing and user interactions
''' from the menus in the game. These include the main menu, game
''' menu and the settings m,enu.
''' </summary>

Module MenuController

    ''' <summary>
    ''' The menu structure for the game.
    ''' </summary>
    ''' <remarks>
    ''' These are the text captions for the menu items.
    ''' </remarks>
    Private ReadOnly _menuStructure As String()() = { _
                New String() {"PLAY", "SETUP", "SCORES", "QUIT"}, _
                New String() {"RETURN", "SURRENDER", "QUIT"}, _
                New String() {"EASY", "MEDIUM", "HARD"}}

    Private Const MENU_TOP As Integer = 575
    Private Const MENU_LEFT As Integer = 30
    Private Const MENU_GAP As Integer = 0
    Private Const BUTTON_WIDTH As Integer = 75
    Private Const BUTTON_HEIGHT As Integer = 15
    Private Const BUTTON_SEP As Integer = BUTTON_WIDTH + MENU_GAP
    Private Const TEXT_OFFSET As Integer = 0

    Private Const MAIN_MENU As Integer = 0
    Private Const GAME_MENU As Integer = 1
    Private Const SETUP_MENU As Integer = 2

    Private Const MAIN_MENU_PLAY_BUTTON As Integer = 0
    Private Const MAIN_MENU_SETUP_BUTTON As Integer = 1
    Private Const MAIN_MENU_TOP_SCORES_BUTTON As Integer = 2
    Private Const MAIN_MENU_QUIT_BUTTON As Integer = 3

    Private Const SETUP_MENU_EASY_BUTTON As Integer = 0
    Private Const SETUP_MENU_MEDIUM_BUTTON As Integer = 1
    Private Const SETUP_MENU_HARD_BUTTON As Integer = 2
    Private Const SETUP_MENU_EXIT_BUTTON As Integer = 3

    Private Const GAME_MENU_RETURN_BUTTON As Integer = 0
    Private Const GAME_MENU_SURRENDER_BUTTON As Integer = 1
    Private Const GAME_MENU_QUIT_BUTTON As Integer = 2

    Private ReadOnly MENU_COLOR As Color = SwinGame.RGBAColor(2, 167, 252, 255)
    Private ReadOnly HIGHLIGHT_COLOR As Color = SwinGame.RGBAColor(1, 57, 86, 255)

    ''' <summary>
    ''' Handles the processing of user input when the main menu is showing
    ''' </summary>
    Public Sub HandleMainMenuInput()
        HandleMenuInput(MAIN_MENU, 0, 0)
    End Sub

    ''' <summary>
    ''' Handles the processing of user input when the main menu is showing
    ''' </summary>
    Public Sub HandleSetupMenuInput()
        Dim handled As Boolean
        handled = HandleMenuInput(SETUP_MENU, 1, 1)

        If Not handled Then
            HandleMenuInput(MAIN_MENU, 0, 0)
        End If
    End Sub

    ''' <summary>
    ''' Handle input in the game menu.
    ''' </summary>
    ''' <remarks>
    ''' Player can return to the game, surrender, or quit entirely
    ''' </remarks>
    Public Sub HandleGameMenuInput()
        HandleMenuInput(GAME_MENU, 0, 0)
    End Sub

    ''' <summary>
    ''' Handles input for the specified menu.
    ''' </summary>
    ''' <param name="menu">the identifier of the menu being processed</param>
    ''' <param name="level">the vertical level of the menu</param>
    ''' <param name="xOffset">the xoffset of the menu</param>
    ''' <returns>false if a clicked missed the buttons. This can be used to check prior menus.</returns>
    Private Function HandleMenuInput(ByVal menu As Integer, ByVal level As Integer, ByVal xOffset As Integer) As Boolean
        If SwinGame.KeyTyped(KeyCode.VK_ESCAPE) Then
            EndCurrentState()
            Return True
        End If

        If SwinGame.MouseClicked(MouseButton.LeftButton) Then
            Dim i as Integer
For i  = 0 To _menuStructure(menu).Length - 1
                'IsMouseOver the i'th button of the menu
                If IsMouseOverMenu(i, level, xOffset) Then
                    PerformMenuAction(menu, i)
                    Return True
                End If
            Next

            If level > 0 Then
                'none clicked - so end this sub menu
                EndCurrentState()
            End If
        End If

        Return False
    End Function

    ''' <summary>
    ''' Draws the main menu to the screen.
    ''' </summary>
    Public Sub DrawMainMenu()
        'Clears the Screen to Black
        'SwinGame.DrawText("Main Menu", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(MAIN_MENU)
    End Sub

    ''' <summary>
    ''' Draws the Game menu to the screen
    ''' </summary>
    Public Sub DrawGameMenu()
        'Clears the Screen to Black
        'SwinGame.DrawText("Paused", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(GAME_MENU)
    End Sub

    ''' <summary>
    ''' Draws the settings menu to the screen.
    ''' </summary>
    ''' <remarks>
    ''' Also shows the main menu
    ''' </remarks>
    Public Sub DrawSettings()
        'Clears the Screen to Black
        'SwinGame.DrawText("Settings", Color.White, GameFont("ArialLarge"), 50, 50)

        DrawButtons(MAIN_MENU)
        DrawButtons(SETUP_MENU, 1, 1)
    End Sub

    ''' <summary>
    ''' Draw the buttons associated with a top level menu.
    ''' </summary>
    ''' <param name="menu">the index of the menu to draw</param>
    Private Sub DrawButtons(ByVal menu As Integer)
        DrawButtons(menu, 0, 0)
    End Sub

    ''' <summary>
    ''' Draws the menu at the indicated level.
    ''' </summary>
    ''' <param name="menu">the menu to draw</param>
    ''' <param name="level">the level (height) of the menu</param>
    ''' <param name="xOffset">the offset of the menu</param>
    ''' <remarks>
    ''' The menu text comes from the _menuStructure field. The level indicates the height
    ''' of the menu, to enable sub menus. The xOffset repositions the menu horizontally
    ''' to allow the submenus to be positioned correctly.
    ''' </remarks>
    Private Sub DrawButtons(ByVal menu As Integer, ByVal level As Integer, ByVal xOffset As Integer)
        Dim btnTop As Integer

        btnTop = MENU_TOP - (MENU_GAP + BUTTON_HEIGHT) * level
        Dim i as Integer
For i  = 0 To _menuStructure(menu).Length - 1
            Dim btnLeft As Integer
            btnLeft = MENU_LEFT + BUTTON_SEP * (i + xOffset)
            'SwinGame.FillRectangle(Color.White, btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT)
            SwinGame.DrawTextLines(_menuStructure(menu)(i), MENU_COLOR, Color.Black, GameFont("Menu"), FontAlignment.AlignCenter, btnLeft + TEXT_OFFSET, btnTop + TEXT_OFFSET, BUTTON_WIDTH, BUTTON_HEIGHT)

            If SwinGame.MouseDown(MouseButton.LeftButton) And IsMouseOverMenu(i, level, xOffset) Then
                SwinGame.DrawRectangle(HIGHLIGHT_COLOR, btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Determined if the mouse is over one of the button in the main menu.
    ''' </summary>
    ''' <param name="button">the index of the button to check</param>
    ''' <returns>true if the mouse is over that button</returns>
    Private Function IsMouseOverButton(ByVal button As Integer) As Boolean
        Return IsMouseOverMenu(button, 0, 0)
    End Function

    ''' <summary>
    ''' Checks if the mouse is over one of the buttons in a menu.
    ''' </summary>
    ''' <param name="button">the index of the button to check</param>
    ''' <param name="level">the level of the menu</param>
    ''' <param name="xOffset">the xOffset of the menu</param>
    ''' <returns>true if the mouse is over the button</returns>
    Private Function IsMouseOverMenu(ByVal button As Integer, ByVal level As Integer, ByVal xOffset As Integer) As Boolean
        Dim btnTop As Integer = MENU_TOP - (MENU_GAP + BUTTON_HEIGHT) * level
        Dim btnLeft As Integer = MENU_LEFT + BUTTON_SEP * (button + xOffset)

        Return IsMouseInRectangle(btnLeft, btnTop, BUTTON_WIDTH, BUTTON_HEIGHT)
    End Function

    ''' <summary>
    ''' A button has been clicked, perform the associated action.
    ''' </summary>
    ''' <param name="menu">the menu that has been clicked</param>
    ''' <param name="button">the index of the button that was clicked</param>
    Private Sub PerformMenuAction(ByVal menu As Integer, ByVal button As Integer)
        Select Case menu
            Case MAIN_MENU
                PerformMainMenuAction(button)
            Case SETUP_MENU
                PerformSetupMenuAction(button)
            Case GAME_MENU
                PerformGameMenuAction(button)
        End Select
    End Sub

    ''' <summary>
    ''' The main menu was clicked, perform the button's action.
    ''' </summary>
    ''' <param name="button">the button pressed</param>
    Private Sub PerformMainMenuAction(ByVal button As Integer)
        Select Case button
            Case MAIN_MENU_PLAY_BUTTON
                StartGame()
            Case MAIN_MENU_SETUP_BUTTON
                AddNewState(GameState.AlteringSettings)
            Case MAIN_MENU_TOP_SCORES_BUTTON
                AddNewState(GameState.ViewingHighScores)
            Case MAIN_MENU_QUIT_BUTTON
                EndCurrentState()
        End Select
    End Sub

    ''' <summary>
    ''' The setup menu was clicked, perform the button's action.
    ''' </summary>
    ''' <param name="button">the button pressed</param>
    Private Sub PerformSetupMenuAction(ByVal button As Integer)
        Select Case button
            Case SETUP_MENU_EASY_BUTTON
                SetDifficulty(AIOption.Hard)
            Case SETUP_MENU_MEDIUM_BUTTON
                SetDifficulty(AIOption.Hard)
            Case SETUP_MENU_HARD_BUTTON
                SetDifficulty(AIOption.Hard)
        End Select
        'Always end state - handles exit button as well
        EndCurrentState()
    End Sub

    ''' <summary>
    ''' The game menu was clicked, perform the button's action.
    ''' </summary>
    ''' <param name="button">the button pressed</param>
    Private Sub PerformGameMenuAction(ByVal button As Integer)
        Select Case button
            Case GAME_MENU_RETURN_BUTTON
                EndCurrentState()
            Case GAME_MENU_SURRENDER_BUTTON
                EndCurrentState() 'end game menu
                EndCurrentState() 'end game
            Case GAME_MENU_QUIT_BUTTON
                AddNewState(GameState.Quitting)
        End Select
    End Sub
End Module
