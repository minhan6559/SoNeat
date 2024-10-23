using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoNeat.src.UI.MainMenu;
using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.GameScreen
{
    // Game over state for the game screen
    public class GameOverState : ISubScreenState
    {
        private GameScreenState _context;

        public GameOverState(GameScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            if (_context.Buttons!["RetryButton"].IsClicked())
            {
                GameScreenState gameScreen = new GameScreenState(_context.EnvironmentManager!);
                ScreenManager.Instance.SetState(gameScreen);
            }

            if (_context.Buttons!["MainMenuButton"].IsClicked())
            {
                MainMenuState mainMenuState = new MainMenuState(_context.EnvironmentManager!);
                ScreenManager.Instance.SetState(mainMenuState);
            }
        }

        public void Draw()
        {
            SplashKit.DrawBitmap(_context.UiBitmaps!["GameOver"], 309, 151);

            foreach (MyButton button in _context.Buttons!.Values)
            {
                button.Draw();
                if (button.IsHovered())
                {
                    SplashKit.DrawBitmap(_context.UiBitmaps!["ChooseArrow"], button.X - 40, button.Y);
                }
            }
        }
    }
}