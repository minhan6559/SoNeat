// GameScreenState.cs
using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;

namespace SoNeat.src.UI.GameScreen
{
    public class GameScreenState : IScreenState
    {
        private IGameState? _currentState;
        public Sonic? Sonic { get; private set; }
        public ObstacleSpawner? ObstacleSpawner { get; private set; }
        public EnvironmentSpawner? EnvironmentSpawner { get; set; }
        public double Score { get; set; }
        public double LastScoreMilestone { get; set; }
        public float GameSpeed { get; set; }
        public float GameSpeedIncrement { get; private set; }
        public Dictionary<string, MyButton>? Buttons { get; private set; }
        public Dictionary<string, Bitmap>? UIBitmaps { get; private set; }

        public void EnterState()
        {
            // Initialize game elements
            GameSpeed = 10;
            Sonic = new Sonic(-110, 509, 634, GameSpeed);
            Sonic.PlayAnimation("Run");
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeedIncrement = 0.5f;
            ObstacleSpawner = new ObstacleSpawner(GameSpeed);
            EnvironmentSpawner ??= new EnvironmentSpawner(GameSpeed);

            // Initialize UI elements
            Buttons = new Dictionary<string, MyButton>
            {
                { "RetryButton", new MyButton("assets/images/GameScreen/retry_btn.png", 562, 318) },
                { "MainMenuButton", new MyButton("assets/images/GameScreen/game_main_menu_btn.png", 512, 368) }
            };

            UIBitmaps = new Dictionary<string, Bitmap>
            {
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png")},
                { "GameOver", SplashKit.LoadBitmap("game_over", "assets/images/GameScreen/game_over.png")}
            };

            // Set initial state
            _currentState = new OpeningSceneState(this);
        }

        public void Update()
        {
            EnvironmentSpawner!.Update();
            Sonic!.Update();
            _currentState!.Update();
            SaveHighScore();
        }

        public void Draw()
        {
            EnvironmentSpawner!.Draw();
            Sonic!.Draw();
            ObstacleSpawner!.Draw();
            DrawScore();
            _currentState!.Draw();
        }

        public void SetState(IGameState newState)
        {
            _currentState = newState;
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            Sonic!.UpdateGameSpeed(gameSpeed);
            EnvironmentSpawner!.UpdateGameSpeed(gameSpeed);
            ObstacleSpawner!.UpdateGameSpeed(gameSpeed);
        }

        private void DrawScore()
        {
            string scoreStr = Math.Floor(Score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 20, 1024, 27);

            // Draw highscore from file
            string highScore = GetHighScore().ToString().PadLeft(5, '0');
            SplashKit.DrawText($"HIGHSCORE:{highScore}", Color.Black, "MainFont", 20, 944, 59);
        }

        private void SaveHighScore()
        {
            string filePath = Utility.NormalizePath("save_contents/highscore.txt");
            if (Score > GetHighScore())
            {
                File.WriteAllText(filePath, Math.Floor(Score).ToString());
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