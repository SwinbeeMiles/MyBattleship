Imports SwinGameSDK

''' <summary>
''' The EndingGameController is responsible for managing the interactions at the end
''' of a game.
''' </summary>

Module EndingGameController

    ''' <summary>
    ''' Draw the end of the game screen, shows the win/lose state
    ''' </summary>
    Public Sub DrawEndOfGame()
        DrawField(ComputerPlayer.PlayerGrid, ComputerPlayer, True)
        DrawSmallField(HumanPlayer.PlayerGrid, HumanPlayer)

        If HumanPlayer.IsDestroyed Then
            SwinGame.DrawTextLines("YOU LOSE!", Color.White, Color.Transparent, GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight())
        Else
            SwinGame.DrawTextLines("-- WINNER --", Color.White, Color.Transparent, GameFont("ArialLarge"), FontAlignment.AlignCenter, 0, 250, SwinGame.ScreenWidth(), SwinGame.ScreenHeight())
        End If
    End Sub

    ''' <summary>
    ''' Handle the input during the end of the game. Any interaction
    ''' will result in it reading in the highsSwinGame.
    ''' </summary>
    Public Sub HandleEndOfGameInput()
        If SwinGame.MouseClicked(MouseButton.LeftButton) _
            OrElse SwinGame.KeyTyped(KeyCode.VK_RETURN) _
            OrElse SwinGame.KeyTyped(KeyCode.VK_ESCAPE) Then
            ReadHighScore(HumanPlayer.Score)
            EndCurrentState()
        End If
    End Sub

End Module
