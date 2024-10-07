using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI.GameScreen
{
    public class OpeningSceneState : ISubScreenState
    {
        private GameScreenState _context;

        public OpeningSceneState(GameScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            _context.Sonic!.X += 5;
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