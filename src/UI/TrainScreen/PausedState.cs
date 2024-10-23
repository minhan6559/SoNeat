using SoNeat.src.UI.MainMenu;
using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    // Paused state for the training screen
    public class PausedState : ISubScreenState
    {
        private TrainScreenState _context;

        public PausedState(TrainScreenState context)
        {
            _context = context;
        }

        public void Update()
        {
            _context.GetModelNameFromTextBox();
            if (_context.Buttons!["ResumeButton"].IsClicked())
            {
                _context.PlayClickSound();
                _context.SuccessMessage = "";
                _context.SetState(new TrainingState(_context));
                _context.ResumeGameSpeed();
            }

            if (_context.Buttons!["MainMenuButton"].IsClicked())
            {
                _context.PlayClickSound();
                Utility.FadeToNewMusic("MainMenuMusic", -1, 500, 1.0f);
                MainMenuState mainMenuState = new MainMenuState(_context.EnvironmentManager!);
                ScreenManager.Instance.SetState(mainMenuState);
            }

            if (_context.Buttons!["SaveModelButton"].IsClicked())
            {
                _context.PlayClickSound();
                _context.SaveNeatModel();
            }
        }

        public void Draw()
        {
            _context.UiBitmaps!["PausedTitle"].Draw(410, 145);
            _context.DrawSuccess();
            _context.Population!.Draw();
            foreach (string buttonName in new string[] { "ResumeButton", "MainMenuButton", "SaveModelButton" })
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