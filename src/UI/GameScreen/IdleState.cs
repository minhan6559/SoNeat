using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashKitSDK;

namespace SoNeat.src.UI.GameScreen
{
    public class IdleState : IGameState
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
                _context.UpdateGameSpeed(_context.GameSpeed);
                _context.SetState(new PlayingState(_context));
            }
        }

        public void Draw()
        {
            SplashKit.DrawText("Press SPACE to start", Color.Black, "MainFont", 24, 400, 300);
        }
    }
}