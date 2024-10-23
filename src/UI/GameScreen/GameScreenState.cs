// GameScreenState.cs
using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;

namespace SoNeat.src.UI.GameScreen
{
    // Game screen state for the game screen
    public class GameScreenState : GameScreenBase
    {
        private Sonic? _sonic;

        public Sonic? Sonic => _sonic;

        public GameScreenState(EnvironmentManager? environmentManager = null)
            : base(environmentManager) { }

        public override void EnterState()
        {
            GameSpeed = 10;
            _sonic = new Sonic(-110, 509, 634, GameSpeed);
            _sonic.PlayAnimation("Run");
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeedIncrement = 0.5f;
            ObstacleManager = new ObstacleManager(GameSpeed);
            EnvironmentManager ??= new EnvironmentManager(GameSpeed);

            InitializeButtons();
            InitializeUiBitmaps();

            CurrentState = new OpeningSceneState(this);
        }

        // Initialize buttons for the game screen
        private void InitializeButtons()
        {
            Buttons = new Dictionary<string, MyButton>
            {
                { "RetryButton", new MyButton("assets/images/GameScreen/retry_btn.png", 562, 318) },
                { "MainMenuButton", new MyButton("assets/images/GameScreen/game_main_menu_btn.png", 512, 368) }
            };
        }

        // Initialize UI bitmaps for the game screen
        private void InitializeUiBitmaps()
        {
            UiBitmaps = new Dictionary<string, Bitmap>
            {
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png")},
                { "GameOver", SplashKit.LoadBitmap("game_over", "assets/images/GameScreen/game_over.png")}
            };
        }

        public override void Update()
        {
            EnvironmentManager!.Update();
            _sonic!.Update();
            CurrentState!.Update();
            SaveHighScore();
        }

        public override void Draw()
        {
            EnvironmentManager!.Draw();
            _sonic!.Draw();
            ObstacleManager!.Draw();
            DrawScore();
            CurrentState!.Draw();
        }

        public override void UpdateGameSpeed(float gameSpeed)
        {
            base.UpdateGameSpeed(gameSpeed);
            _sonic!.UpdateGameSpeed(gameSpeed);
        }

        private void DrawScore()
        {
            string scoreStr = Math.Floor(Score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 20, 1024, 27);

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

        public override void ExitState()
        {
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}