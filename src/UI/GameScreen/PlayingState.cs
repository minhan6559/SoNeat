using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI.GameScreen
{
    public class PlayingState : IGameState
    {
        private GameScreenState _context;

        public PlayingState(GameScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            _context.Sonic!.HandleInput();
            _context.Score += _context.GameSpeed / 60;
            if (Math.Floor(_context.Score) >= _context.LastScoreMilestone + 100)
            {
                _context.LastScoreMilestone = Math.Floor(_context.Score);
                _context.UpdateGameSpeed(_context.GameSpeed + _context.GameSpeedIncrement);
            }

            _context.ObstacleManager!.Update(_context.Sonic);
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