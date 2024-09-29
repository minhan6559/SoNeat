using SoNeat.src.UI.MainMenu;
using SoNeat.src.Utils;
using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    public class PausedState : ITrainState
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
                _context.SuccessMessage = "";
                _context.SetState(new TrainingState(_context));
                _context.UpdateGameSpeed(_context.GameSpeed);
            }

            if (_context.Buttons!["MainMenuButton"].IsClicked())
            {
                MainMenuState mainMenuState = new MainMenuState();
                mainMenuState.EnvironmentSpawner = _context.EnvironmentSpawner;
                ScreenManager.Instance.SetState(mainMenuState);
            }

            if (_context.Buttons!["SaveModelButton"].IsClicked())
            {
                _context.Neat!.SerializeToJson($"save_contents/{_context.ModelName}.json");
                _context.SuccessMessage = "Model saved successfully";
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