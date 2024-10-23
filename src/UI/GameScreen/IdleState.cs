using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.GameScreen
{
    // Waiting state for the game screen
    public class IdleState : ISubScreenState
    {
        private GameScreenState _context;

        public IdleState(GameScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            if (SplashKit.KeyTyped(KeyCode.SpaceKey))
            {
                _context.Sonic!.IsIdle = false;
                _context.ResumeGameSpeed();

                Utility.FadeToNewMusic("GameMusic", -1, 500, 0.4f);
                _context.SetState(new PlayingState(_context));
            }
        }

        public void Draw()
        {
            SplashKit.DrawText("Press SPACE to start", Color.Black, "MainFont", 24, 400, 300);
        }
    }
}