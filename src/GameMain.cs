using System;
using SwinGameSDK;

namespace MyGame
{
    public class GameMain
    {
        public static void Main()
        {
            //Open the game window
            SwinGame.OpenGraphicsWindow("GameMain", 800, 600);
            SwinGame.ShowSwinGameSplashScreen();
            
			//Load Resources
			GameResources.LoadResources();

			SwinGame.PlayMusic (GameResources.GameMusic ("Background"));

            //Run the game loop
            while(false == SwinGame.WindowCloseRequested())
            {
                //Fetch the next batch of UI interaction
                SwinGame.ProcessEvents();
                
                //Clear the screen and draw the framerate
                SwinGame.ClearScreen(Color.White);
                SwinGame.DrawFramerate(0,0);
                
                //Draw onto the screen
                SwinGame.RefreshScreen(60);

				GameController.HandleUserInput ();
				GameController.DrawScreen ();
            }

			SwinGame.StopMusic ();

			GameResources.FreeResources ();
        }
    }
}