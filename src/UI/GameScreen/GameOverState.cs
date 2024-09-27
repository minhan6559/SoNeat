using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoNeat.src.UI.MainMenu;
using SplashKitSDK;

namespace SoNeat.src.UI.GameScreen
{
    public class GameOverState : IGameState
    {
        private GameScreenState _context;

        public GameOverState(GameScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            if (_context.RetryBtn!.IsClicked())
            {
                GameScreenState gameScreen = new GameScreenState();
                gameScreen.EnvironmentManager = _context.EnvironmentManager;
                ScreenManager.Instance.SetState(gameScreen);
            }

            if (_context.MainMenuBtn!.IsClicked())
            {
                ScreenManager.Instance.SetState(new MainMenuState());
            }
        }

        public void Draw()
        {
            SplashKit.DrawBitmap(_context.GameOverBitmap!, 305, 151);

            _context.RetryBtn!.Draw();
            if (_context.RetryBtn.IsHovered())
            {
                _context.ChooseArrow!.Draw(_context.RetryBtn.X - 40, _context.RetryBtn.Y);
            }

            _context.MainMenuBtn!.Draw();
            if (_context.MainMenuBtn.IsHovered())
            {
                _context.ChooseArrow!.Draw(_context.MainMenuBtn.X - 40, _context.MainMenuBtn.Y);
            }
        }
    }
}