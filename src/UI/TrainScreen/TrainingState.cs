using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    public class TrainingState : ISubScreenState
    {
        private TrainScreenState _context;

        public TrainingState(TrainScreenState context)
        {
            Utility.FadeToNewMusic("GameMusic", 500, 0.4f);
            _context = context;
        }

        public void Update()
        {
            _context.UpdateScore();
            _context.CheckUpdateGameSpeed();
            _context.UpdateFrameRate();

            _context.ObstacleManager!.Update(_context.Population!);
            _context.Population!.Update(_context.ObstacleManager.Obstacles);

            if (_context.IsAllDead())
                _context.Reset();


            if (SplashKit.KeyTyped(KeyCode.EscapeKey))
            {
                ScreenManager.FrameRate = 60;
                _context.UpdateGameSpeed(0);
                _context.SetState(new PausedState(_context));
            }

            if (SplashKit.KeyTyped(KeyCode.FKey))
            {
                _context.ToggleFastForward();
            }
        }

        public void Draw()
        {
            _context.DrawKeyboardShorcut();
            _context.DrawTrainingInfo();
            _context.Population!.Draw();
            _context.DrawBestNetwork();
        }
    }
}