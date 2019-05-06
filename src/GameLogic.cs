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

			GameController.PlayMusic ();

            //Run the game loop
			while(!(true == SwinGame.WindowCloseRequested() || GameController.CurrentState == GameState.Quitting))
			{

				GameController.HandleUserInput ();
				GameController.DrawScreen ();
            }

			SwinGame.StopMusic ();

			GameResources.FreeResources ();
        }
    }
}