using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI.GameScreen
{
    // Opening scene state for the game screen
    public class OpeningSceneState : ISubScreenState
    {
        private GameScreenState _context;

        public OpeningSceneState(GameScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            // Move Sonic to the right
            _context.Sonic!.X += 5;

            // If Sonic reaches the end of the screen, stop moving
            if (_context.Sonic.X >= 52)
            {
                _context.Sonic.X = 52;
                _context.Sonic.PlayAnimation("Idle");
                _context.SetState(new IdleState(_context));
            }
        }

        public void Draw() { }
    }
}