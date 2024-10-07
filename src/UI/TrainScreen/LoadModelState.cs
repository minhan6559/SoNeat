using SoNeat.src.NEAT;
using SoNeat.src.UI.MainMenu;
using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    public class LoadModelState : ISubScreenState
    {
        private TrainScreenState _context;

        public LoadModelState(TrainScreenState context)
        {
            _context = context;
            _context.UpdateGameSpeed(0);
        }

        public void Update()
        {
            _context.GetModelNameFromTextBox();
            if (_context.Buttons!["ChooseModelButton"].IsClicked() && _context.CheckValidModelName())
            {
                _context.ResumeGameSpeed();
                _context.LoadNeatModel();
                _context.SetState(new TrainingState(_context));
            }

            if (_context.Buttons!["RetrainButton"].IsClicked())
            {
                _context.ResumeGameSpeed();
                _context.InitializeNeatModel();
                _context.SetState(new TrainingState(_context));
            }

            if (_context.Buttons!["MainMenuButton"].IsClicked())
            {
                MainMenuState mainMenuState = new MainMenuState(_context.EnvironmentManager!);
                ScreenManager.Instance.SetState(mainMenuState);
            }
        }

        public void Draw()
        {
            _context.UiBitmaps!["LoadModelTitle"].Draw(278, 145);
            _context.DrawError();
            foreach (string buttonName in new string[] { "ChooseModelButton", "MainMenuButton", "RetrainButton" })
            {
                MyButton button = _context.Buttons![buttonName];
                button.Draw();
                if (button.IsHovered())
                {
                    SplashKit.DrawBitmap(_context.UiBitmaps["ChooseArrow"], button.X - 40, button.Y);
                }
            }
            SplashKit.DrawInterface();
        }
    }
}