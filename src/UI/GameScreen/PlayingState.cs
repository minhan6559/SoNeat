using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoNeat.src.Utils;

namespace SoNeat.src.UI.GameScreen
{
    // Playing state for the game screen
    public class PlayingState : ISubScreenState
    {
        private GameScreenState _context;

        public PlayingState(GameScreenState context)
        {
            Utility.FadeToNewMusic("GameMusic", 500, 0.4f);
            _context = context;
        }

        public void Update()
        {
            _context.Sonic!.HandleInput();
            _context.UpdateScore();
            _context.CheckUpdateGameSpeed();

            _context.ObstacleManager!.Update(_context.Sonic);

            // Check if Sonic is dead and change state to GameOverState
            if (_context.Sonic.IsDead)
            {
                _context.Sonic.PlayAnimation("Dead");
                _context.UpdateGameSpeed(0);
                _context.SetState(new GameOverState(_context));
            }
        }

        public void Draw() { }
    }
}