using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.UI
{
    // Singleton class for managing screens
    public class ScreenManager
    {
        private static ScreenManager? _instance;
        private static readonly object _lock = new object();
        private IScreenState? _currentState;

        // Global FrameRate property
        private static int _frameRate = 60; // Default value
        public static int FrameRate
        {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

        // Private constructor to prevent instantiation
        private ScreenManager() { }

        // Public static method to get the instance of ScreenManager
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ScreenManager();
                    }
                }
                return _instance;
            }
        }

        public void SetState(IScreenState state)
        {
            if (_currentState != null)
                _currentState.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Draw()
        {
            _currentState?.Draw();
        }

        public static void LoadResources()
        {
            LoadInterfaceResources();
            LoadAudioResources();
            LoadInterfaceResources();
        }

        private static void LoadAudioResources()
        {
            SplashKit.LoadMusic("GameMusic", Utility.NormalizePath("assets/sounds/game_music.mp3"));
            SplashKit.LoadMusic("GameOverMusic", Utility.NormalizePath("assets/sounds/game_over.mp3"));
            SplashKit.LoadMusic("MainMenuMusic", Utility.NormalizePath("assets/sounds/main_menu_music.wav"));
            SplashKit.LoadMusic("TrainMusic", Utility.NormalizePath("assets/sounds/train_music.mp3"));

            SplashKit.LoadSoundEffect("JumpSoundEffect", Utility.NormalizePath("assets/sounds/jump.mp3"));
            SplashKit.LoadSoundEffect("ClickSoundEffect", Utility.NormalizePath("assets/sounds/click.wav"));
            SplashKit.LoadSoundEffect("CheckpointSoundEffect", Utility.NormalizePath("assets/sounds/checkpoint.wav"));
            SplashKit.LoadSoundEffect("IncorrectSoundEffect", Utility.NormalizePath("assets/sounds/incorrect.wav"));
        }

        private static void LoadInterfaceResources()
        {
            SplashKit.LoadFont("MainFont", Utility.NormalizePath("assets/fonts/PressStart2P.ttf"));
            SplashKit.SetInterfaceFont("MainFont");
            SplashKit.SetInterfaceFontSize(12);
        }

        public static void FreeResources()
        {
            SplashKit.FreeAllBitmaps();
            SplashKit.FreeAllFonts();
            SplashKit.FreeAllMusic();
            SplashKit.FreeAllSoundEffects();
        }
    }

}