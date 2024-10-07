using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashKitSDK;

namespace SoNeat.src.UI.GameScreen
{
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
                _context.SetState(new PlayingState(_context));
            }
        }

        public void Draw()
        {
            SplashKit.DrawText("Press SPACE to start", Color.Black, "MainFont", 24, 400, 300);
        }
    }
}