static class GameLogic
{
    public static void Main()
    {
        // Opens a new Graphics Window
        SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);

        // Load Resources
        LoadResources();

        SwinGame.PlayMusic(GameMusic("Background"));

        // Game Loop
        do
        {
            HandleUserInput();
            DrawScreen();
        }
        while (!SwinGame.WindowCloseRequested() == true | CurrentState == GameState.Quitting);

        SwinGame.StopMusic();

        // Free Resources and Close Audio, to end the program.
        FreeResources();
    }
}
