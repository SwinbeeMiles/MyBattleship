using System.IO;
using SwinGameSDK;
using System.Collections.Generic;

static class InstructionsController
{
	public static void HandleInstructionsInput ()
	{
		if (SwinGame.MouseClicked (MouseButton.LeftButton) || SwinGame.KeyTyped (KeyCode.vk_ESCAPE) || SwinGame.KeyTyped (KeyCode.vk_RETURN)) {
			GameController.EndCurrentState ();
		}
	}
}