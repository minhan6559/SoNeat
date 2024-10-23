using SoNeat.src.NEAT;
using SoNeat.src.UI.MainMenu;
using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    // Load model state for the training screen
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
            // Check if the user has chosen a model name and load the model
            _context.GetModelNameFromTextBox();

            // Check if the user has clicked the buttons and the model name is valid
            if (_context.Buttons!["ChooseModelButton"].IsClicked() && _context.CheckValidModelName())
            {
                _context.PlayClickSound();
                _context.ResumeGameSpeed();
                _context.LoadNeatModel();
                Utility.FadeToNewMusic("TrainMusic", -1, 500, 0.4f);
                _context.SetState(new TrainingState(_context));
            }

            if (_context.Buttons!["RetrainButton"].IsClicked())
            {
                _context.PlayClickSound();
                _context.ResumeGameSpeed();
                _context.InitializeNeatModel();
                Utility.FadeToNewMusic("TrainMusic", -1, 500, 0.4f);
                _context.SetState(new TrainingState(_context));
            }

            if (_context.Buttons!["MainMenuButton"].IsClicked())
            {
                _context.PlayClickSound();
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