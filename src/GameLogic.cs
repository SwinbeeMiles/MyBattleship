using SwinGameSDK;

namespace MyGame
{
    public class GameLogic
    {
        public static void Main()
        {
            //Open the game window
            SwinGame.OpenGraphicsWindow("Battle Ships", 800, 600);
            //SwinGame.ShowSwinGameSplashScreen();
            
			//Load Resources
			GameResources.LoadResources ();

			SwinGame.PlayMusic (GameResources.GameMusic ("Background"));

            //Run the game loop
			while(false == SwinGame.WindowCloseRequested() || GameController.CurrentState != GameState.Quitting)
			{

				GameController.HandleUserInput ();
				GameController.DrawScreen ();
            }

			SwinGame.StopMusic ();

			GameResources.FreeResources ();
        }
    }
}