// GameScreenState.cs
using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;

namespace SoNeat.src.UI.GameScreen
{
    public class GameScreenState : IScreenState
    {
        private ISubScreenState? _currentState;
        private Sonic? _sonic;
        private ObstacleManager? _obstacleManager;
        private EnvironmentManager? _environmentManager;
        private double _score, _lastScoreMilestone;
        private float _gameSpeed, _gameSpeedIncrement;
        private Dictionary<string, MyButton>? _buttons;
        private Dictionary<string, Bitmap>? _uiBitmaps;

        public Sonic? Sonic => _sonic;

        public ObstacleManager? ObstacleManager => _obstacleManager;

        public EnvironmentManager? EnvironmentManager => _environmentManager;

        public Dictionary<string, MyButton>? Buttons => _buttons;

        public Dictionary<string, Bitmap>? UIBitmaps => _uiBitmaps;

        public GameScreenState(EnvironmentManager? environmentManager = null)
        {
            _environmentManager = environmentManager;
        }

        public void EnterState()
        {
            // Initialize game elements
            _gameSpeed = 10;
            _sonic = new Sonic(-110, 509, 634, _gameSpeed);
            _sonic.PlayAnimation("Run");
            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeedIncrement = 0.5f;
            _obstacleManager = new ObstacleManager(_gameSpeed);
            _environmentManager ??= new EnvironmentManager(_gameSpeed);

            // Initialize UI elements
            _buttons = new Dictionary<string, MyButton>
            {
                { "RetryButton", new MyButton("assets/images/GameScreen/retry_btn.png", 562, 318) },
                { "MainMenuButton", new MyButton("assets/images/GameScreen/game_main_menu_btn.png", 512, 368) }
            };

            _uiBitmaps = new Dictionary<string, Bitmap>
            {
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png")},
                { "GameOver", SplashKit.LoadBitmap("game_over", "assets/images/GameScreen/game_over.png")}
            };

            // Set initial state
            _currentState = new OpeningSceneState(this);
        }

        public void Update()
        {
            _environmentManager!.Update();
            _sonic!.Update();
            _currentState!.Update();
            SaveHighScore();
        }

        public void Draw()
        {
            _environmentManager!.Draw();
            _sonic!.Draw();
            _obstacleManager!.Draw();
            DrawScore();
            _currentState!.Draw();
        }

        public void SetState(ISubScreenState newState)
        {
            _currentState = newState;
        }

        public void UpdateGameSpeed(float _gameSpeed)
        {
            _sonic!.UpdateGameSpeed(_gameSpeed);
            _environmentManager!.UpdateGameSpeed(_gameSpeed);
            _obstacleManager!.UpdateGameSpeed(_gameSpeed);
        }

        private void IncreaseGameSpeed()
        {
            _gameSpeed += _gameSpeedIncrement;
            UpdateGameSpeed(_gameSpeed);
        }

        public void CheckUpdateGameSpeed()
        {
            if (Math.Floor(_score) >= _lastScoreMilestone + 100)
            {
                _lastScoreMilestone = Math.Floor(_score);
                IncreaseGameSpeed();
            }
        }

        public void ResumeGameSpeed()
        {
            UpdateGameSpeed(_gameSpeed);
        }

        public void UpdateScore()
        {
            _score += _gameSpeed / 60;
        }

        private void DrawScore()
        {
            string scoreStr = Math.Floor(_score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 20, 1024, 27);

            // Draw highscore from file
            string highScore = GetHighScore().ToString().PadLeft(5, '0');
            SplashKit.DrawText($"HIGHSCORE:{highScore}", Color.Black, "MainFont", 20, 944, 59);
        }

        private void SaveHighScore()
        {
            string filePath = Utility.NormalizePath("save_contents/highscore.txt");
            if (_score > GetHighScore())
            {
                File.WriteAllText(filePath, Math.Floor(_score).ToString());
            }
        }

        private double GetHighScore()
        {
            string filePath = Utility.NormalizePath("save_contents/highscore.txt");
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, "0");
            }

            return double.Parse(File.ReadAllText(filePath));
        }

        public void ExitState()
        {
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}