Imports SwinGameSDK

''' <summary>
''' The GameController is responsible for controlling the game,
''' managing user input, and displaying the current state of the
''' game.
''' </summary>
Public Module GameController

    Private _theGame As BattleShipsGame
    Private _human As Player
    Private _ai As AIPlayer

    Private _state As Stack(Of GameState) = New Stack(Of GameState)()

    Private _aiSetting As AIOption

    ''' <summary>
    ''' Returns the current state of the game, indicating which screen is
    ''' currently being used
    ''' </summary>
    ''' <value>The current state</value>
    ''' <returns>The current state</returns>
    Public ReadOnly Property CurrentState() As GameState
        Get
            Return _state.Peek()
        End Get
    End Property

    ''' <summary>
    ''' Returns the human player.
    ''' </summary>
    ''' <value>the human player</value>
    ''' <returns>the human player</returns>
    Public ReadOnly Property HumanPlayer() As Player
        Get
            Return _human
        End Get
    End Property

    ''' <summary>
    ''' Returns the computer player.
    ''' </summary>
    ''' <value>the computer player</value>
    ''' <returns>the conputer player</returns>
    Public ReadOnly Property ComputerPlayer() As Player
        Get
            Return _ai
        End Get
    End Property

    Sub New()
        'bottom state will be quitting. If player exits main menu then the game is over
        _state.Push(GameState.Quitting)

        'at the start the player is viewing the main menu
        _state.Push(GameState.ViewingMainMenu)
    End Sub

    ''' <summary>
    ''' Starts a new game.
    ''' </summary>
    ''' <remarks>
    ''' Creates an AI player based upon the _aiSetting.
    ''' </remarks>
    Public Sub StartGame()
        If _theGame IsNot Nothing Then EndGame()

        'Create the game
        _theGame = New BattleShipsGame()

        'create the players
        Select Case _aiSetting
            Case AIOption.Medium
                _ai = New AIMediumPlayer(_theGame)
            Case AIOption.Hard
                _ai = New AIHardPlayer(_theGame)
            Case Else
                _ai = New AIHardPlayer(_theGame)
        End Select

        _human = New Player(_theGame)

        'AddHandler _human.PlayerGrid.Changed, AddressOf GridChanged
        AddHandler _ai.PlayerGrid.Changed, AddressOf GridChanged
        AddHandler _theGame.AttackCompleted, AddressOf AttackCompleted

        AddNewState(GameState.Deploying)
    End Sub

    ''' <summary>
    ''' Stops listening to the old game once a new game is started
    ''' </summary>

    Private Sub EndGame()
        'RemoveHandler _human.PlayerGrid.Changed, AddressOf GridChanged
        RemoveHandler _ai.PlayerGrid.Changed, AddressOf GridChanged
        RemoveHandler _theGame.AttackCompleted, AddressOf AttackCompleted
    End Sub

    ''' <summary>
    ''' Listens to the game grids for any changes and redraws the screen
    ''' when the grids change
    ''' </summary>
    ''' <param name="sender">the grid that changed</param>
    ''' <param name="args">not used</param>
    Private Sub GridChanged(ByVal sender As Object, ByVal args As EventArgs)
        DrawScreen()
        SwinGame.RefreshScreen()
    End Sub

    Private Sub PlayHitSequence(ByVal row As Integer, ByVal column As Integer, ByVal showAnimation As Boolean)
        If showAnimation Then
            AddExplosion(row, column)
        End If

        Audio.PlaySoundEffect(GameSound("Hit"))

        DrawAnimationSequence()
    End Sub

    Private Sub PlayMissSequence(ByVal row As Integer, ByVal column As Integer, ByVal showAnimation As Boolean)
        If showAnimation Then
            AddSplash(row, column)
        End If

        Audio.PlaySoundEffect(GameSound("Miss"))

        DrawAnimationSequence()
    End Sub

    ''' <summary>
    ''' Listens for attacks to be completed.
    ''' </summary>
    ''' <param name="sender">the game</param>
    ''' <param name="result">the result of the attack</param>
    ''' <remarks>
    ''' Displays a message, plays sound and redraws the screen
    ''' </remarks>
    Private Sub AttackCompleted(ByVal sender As Object, ByVal result As AttackResult)
        Dim isHuman As Boolean
        isHuman = _theGame.Player Is HumanPlayer

        If isHuman Then
            Message = "You " & result.ToString()
        Else
            Message = "The AI " & result.ToString()
        End If

        Select Case result.Value
            Case ResultOfAttack.Destroyed
                PlayHitSequence(result.Row, result.Column, isHuman)
                Audio.PlaySoundEffect(GameSound("Sink"))

            Case ResultOfAttack.GameOver
                PlayHitSequence(result.Row, result.Column, isHuman)
                Audio.PlaySoundEffect(GameSound("Sink"))

                While Audio.SoundEffectPlaying(GameSound("Sink"))
                    SwinGame.Delay(10)
                    SwinGame.RefreshScreen()
                End While

                If HumanPlayer.IsDestroyed Then
                    Audio.PlaySoundEffect(GameSound("Lose"))
                Else
                    Audio.PlaySoundEffect(GameSound("Winner"))
                End If

            Case ResultOfAttack.Hit
                PlayHitSequence(result.Row, result.Column, isHuman)
            Case ResultOfAttack.Miss
                PlayMissSequence(result.Row, result.Column, isHuman)
            Case ResultOfAttack.ShotAlready
                Audio.PlaySoundEffect(GameSound("Error"))
        End Select
    End Sub

    ''' <summary>
    ''' Completes the deployment phase of the game and
    ''' switches to the battle mode (Discovering state)
    ''' </summary>
    ''' <remarks>
    ''' This adds the players to the game before switching
    ''' state.
    ''' </remarks>
    Public Sub EndDeployment()
        'deploy the players
        _theGame.AddDeployedPlayer(_human)
        _theGame.AddDeployedPlayer(_ai)

        SwitchState(GameState.Discovering)
    End Sub

    ''' <summary>
    ''' Gets the player to attack the indicated row and column.
    ''' </summary>
    ''' <param name="row">the row to attack</param>
    ''' <param name="col">the column to attack</param>
    ''' <remarks>
    ''' Checks the attack result once the attack is complete
    ''' </remarks>
    Public Sub Attack(ByVal row As Integer, ByVal col As Integer)
        Dim result As AttackResult
        result = _theGame.Shoot(row, col)
        CheckAttackResult(result)
    End Sub

    ''' <summary>
    ''' Gets the AI to attack.
    ''' </summary>
    ''' <remarks>
    ''' Checks the attack result once the attack is complete.
    ''' </remarks>
    Private Sub AIAttack()
        Dim result As AttackResult
        result = _theGame.Player.Attack()
        CheckAttackResult(result)
    End Sub

    ''' <summary>
    ''' Checks the results of the attack and switches to
    ''' Ending the Game if the result was game over.
    ''' </summary>
    ''' <param name="result">the result of the last
    ''' attack</param>
    ''' <remarks>Gets the AI to attack if the result switched
    ''' to the AI player.</remarks>
    Private Sub CheckAttackResult(ByVal result As AttackResult)
        Select Case result.Value
            Case ResultOfAttack.Miss
                If _theGame.Player Is ComputerPlayer Then AIAttack()
            Case ResultOfAttack.GameOver
                SwitchState(GameState.EndingGame)
        End Select
    End Sub

    ''' <summary>
    ''' Handles the user SwinGame.
    ''' </summary>
    ''' <remarks>
    ''' Reads key and mouse input and converts these into
    ''' actions for the game to perform. The actions
    ''' performed depend upon the state of the game.
    ''' </remarks>
    Public Sub HandleUserInput()
        'Read incoming input events
        SwinGame.ProcessEvents()

        Select Case CurrentState
            Case GameState.ViewingMainMenu
                HandleMainMenuInput()
            Case GameState.ViewingGameMenu
                HandleGameMenuInput()
            Case GameState.AlteringSettings
                HandleSetupMenuInput()
            Case GameState.Deploying
                HandleDeploymentInput()
            Case GameState.Discovering
                HandleDiscoveryInput()
            Case GameState.EndingGame
                HandleEndOfGameInput()
            Case GameState.ViewingHighScores
                HandleHighScoreInput()
        End Select

        UpdateAnimations()
    End Sub

    ''' <summary>
    ''' Draws the current state of the game to the screen.
    ''' </summary>
    ''' <remarks>
    ''' What is drawn depends upon the state of the game.
    ''' </remarks>
    Public Sub DrawScreen()
        DrawBackground()

        Select Case CurrentState
            Case GameState.ViewingMainMenu
                DrawMainMenu()
            Case GameState.ViewingGameMenu
                DrawGameMenu()
            Case GameState.AlteringSettings
                DrawSettings()
            Case GameState.Deploying
                DrawDeployment()
            Case GameState.Discovering
                DrawDiscovery()
            Case GameState.EndingGame
                DrawEndOfGame()
            Case GameState.ViewingHighScores
                DrawHighScores()
        End Select

        DrawAnimations()

        SwinGame.RefreshScreen()
    End Sub

    ''' <summary>
    ''' Move the game to a new state. The current state is maintained
    ''' so that it can be returned to.
    ''' </summary>
    ''' <param name="state">the new game state</param>
    Public Sub AddNewState(ByVal state As GameState)
        _state.Push(state)
        Message = ""
    End Sub

    ''' <summary>
    ''' End the current state and add in the new state.
    ''' </summary>
    ''' <param name="newState">the new state of the game</param>
    Public Sub SwitchState(ByVal newState As GameState)
        EndCurrentState()
        AddNewState(newState)
    End Sub

    ''' <summary>
    ''' Ends the current state, returning to the prior state
    ''' </summary>
    Public Sub EndCurrentState()
        _state.Pop()
    End Sub

    ''' <summary>
    ''' Sets the difficulty for the next level of the game.
    ''' </summary>
    ''' <param name="setting">the new difficulty level</param>
    Public Sub SetDifficulty(ByVal setting As AIOption)
        _aiSetting = setting
    End Sub

End Module
