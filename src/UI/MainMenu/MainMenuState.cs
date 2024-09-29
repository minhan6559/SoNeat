using SplashKitSDK;
using SoNeat.src.Utils;
using SoNeat.src.GameLogic;
using SoNeat.src.UI.GameScreen;
using SoNeat.src.UI.TrainScreen;

namespace SoNeat.src.UI.MainMenu
{
    public class MainMenuState : IScreenState
    {
        private Dictionary<string, MyButton>? _buttons;
        private Dictionary<string, Bitmap>? _uiBitmaps;
        private EnvironmentSpawner? _environmentSpawner;

        public EnvironmentSpawner? EnvironmentSpawner
        {
            get => _environmentSpawner;
            set => _environmentSpawner = value;
        }

        public void EnterState()
        {
            _buttons = new Dictionary<string, MyButton>
            {
                { "PlayButton", new MyButton("assets/images/MainMenu/play.png", 581, 322) },
                { "TrainButton", new MyButton("assets/images/MainMenu/train.png", 537, 370) },
                { "ExitButton", new MyButton("assets/images/MainMenu/exit.png", 581, 418) }
            };

            _uiBitmaps = new Dictionary<string, Bitmap>()
            {
                { "Title", SplashKit.LoadBitmap("title", "assets/images/MainMenu/title.png") },
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/MainMenu/choose_arrow.png") }
            };

            _environmentSpawner = new EnvironmentSpawner(0);
        }

        public void Update()
        {
            _environmentSpawner!.Update();

            if (_buttons!["PlayButton"].IsClicked())
            {
                GameScreenState gameScreen = new GameScreenState();
                gameScreen.EnvironmentSpawner = _environmentSpawner!;
                ScreenManager.Instance.SetState(gameScreen);
            }

            if (_buttons!["TrainButton"].IsClicked())
            {
                TrainScreenState trainScreen = new TrainScreenState();
                trainScreen.EnvironmentSpawner = _environmentSpawner!;
                ScreenManager.Instance.SetState(trainScreen);
            }

            if (_buttons!["ExitButton"].IsClicked())
            {
                SplashKit.CloseAllWindows();
            }
        }

        public void Draw()
        {
            _environmentSpawner!.Draw();
            _uiBitmaps!["Title"].Draw(415, 161);

            // Loop through the buttons and draw them
            foreach (MyButton button in _buttons!.Values)
            {
                button.Draw();
                if (button.IsHovered())
                {
                    _uiBitmaps!["ChooseArrow"].Draw(button.X - 40, button.Y);
                }
            }
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Main Menu State");
        }
    }
}