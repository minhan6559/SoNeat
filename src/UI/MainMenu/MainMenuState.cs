using SplashKitSDK;
using SoNeat.src.Utils;
using SoNeat.src.GameLogic;
using SoNeat.src.UI.GameScreen;
using SoNeat.src.UI.TrainScreen;

namespace SoNeat.src.UI.MainMenu
{
    // Main menu state for the program
    public class MainMenuState : IScreenState
    {
        private Dictionary<string, MyButton>? _buttons;
        private Dictionary<string, Bitmap>? _uiBitmaps;
        private EnvironmentManager? _environmentManager;

        public EnvironmentManager? EnvironmentManager => _environmentManager;

        public MainMenuState(EnvironmentManager? environmentManager = null)
        {
            _environmentManager = environmentManager;
        }

        public void EnterState()
        {
            InitializeUiBitmaps();
            InitializeButtons();
            LoadAudioResources();

            Utility.FadeToNewMusic("MainMenuMusic", 500, 1.0f);

            _environmentManager = new EnvironmentManager(0);
        }

        private void InitializeUiBitmaps()
        {
            _uiBitmaps = new Dictionary<string, Bitmap>()
            {
                { "Title", SplashKit.LoadBitmap("title", "assets/images/MainMenu/title.png") },
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/MainMenu/choose_arrow.png") }
            };
        }

        private void InitializeButtons()
        {
            _buttons = new Dictionary<string, MyButton>
            {
                { "PlayButton", new MyButton("assets/images/MainMenu/play.png", 581, 322) },
                { "TrainButton", new MyButton("assets/images/MainMenu/train.png", 537, 370) },
                { "ExitButton", new MyButton("assets/images/MainMenu/exit.png", 581, 418) }
            };
        }

        private void LoadAudioResources()
        {
            SplashKit.LoadMusic("GameMusic", Utility.NormalizePath("assets/sounds/game_music.mp3"));
            SplashKit.LoadMusic("GameOverMusic", Utility.NormalizePath("assets/sounds/game_over.mp3"));
            SplashKit.LoadMusic("MainMenuMusic", Utility.NormalizePath("assets/sounds/main_menu_music.wav"));
            SplashKit.LoadSoundEffect("JumpSoundEffect", Utility.NormalizePath("assets/sounds/jump.mp3"));
        }

        public void Update()
        {
            _environmentManager!.Update();

            if (_buttons!["PlayButton"].IsClicked())
            {
                GameScreenState gameScreen = new GameScreenState(_environmentManager);
                ScreenManager.Instance.SetState(gameScreen);
            }

            if (_buttons!["TrainButton"].IsClicked())
            {
                TrainScreenState trainScreen = new TrainScreenState(_environmentManager);
                ScreenManager.Instance.SetState(trainScreen);
            }

            if (_buttons!["ExitButton"].IsClicked())
            {
                SplashKit.CloseAllWindows();
                FreeResources();
            }
        }

        public void Draw()
        {
            _environmentManager!.Draw();
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

        private void FreeResources()
        {
            SplashKit.FreeAllBitmaps();
            SplashKit.FreeAllFonts();
            SplashKit.FreeAllMusic();
            SplashKit.FreeAllSoundEffects();
        }
    }
}