using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    public class TrainingState : ITrainState
    {
        private TrainScreenState _context;

        public TrainingState(TrainScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            _context.Score += _context.GameSpeed / 60;
            if (Math.Floor(_context.Score) >= _context.LastScoreMilestone + 100)
            {
                _context.LastScoreMilestone = Math.Floor(_context.Score);
                _context.GameSpeed += _context.GameSpeedIncrement;
                _context.UpdateGameSpeed(_context.GameSpeed);
            }

            _context.ObstacleManager!.Update(_context.Population!, _context.Score);
            _context.Population!.Update(_context.ObstacleManager.Obstacles);

            if (_context.Population.Alives <= 0)
                _context.Reset();

            if (_context.IsFastForward)
            {
                ScreenManager.FrameRate = 600;
            }
            else
            {
                ScreenManager.FrameRate = 60;
            }

            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
            {
                ScreenManager.FrameRate = 60;
                _context.UpdateGameSpeed(0);
                _context.SetState(new PausedState(_context));
            }

            if (SplashKit.KeyTyped(KeyCode.FKey))
            {
                _context.IsFastForward = !_context.IsFastForward;
            }
        }

        public void Draw()
        {
            _context.DrawTrainingInfo();
            _context.Population!.Draw();
            _context.NetworkDrawer!.Draw(_context.Neat!.BestAgent.Genome);
        }
    }
}