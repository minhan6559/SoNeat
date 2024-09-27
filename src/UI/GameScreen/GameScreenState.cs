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
        public ObstacleManager? ObstacleManager { get; private set; }
        public EnvironmentManager? EnvironmentManager { get; set; }
        public double Score { get; set; }
        public double LastScoreMilestone { get; set; }
        public float GameSpeed { get; set; }
        public float GameSpeedIncrement { get; private set; }
        public MyButton? RetryBtn { get; private set; }
        public MyButton? MainMenuBtn { get; private set; }
        public Bitmap? ChooseArrow { get; private set; }
        public Bitmap? GameOverBitmap { get; private set; }

        public void EnterState()
        {
            // Initialize game elements
            GameSpeed = 10;
            Sonic = new Sonic(-110, 509, 634, GameSpeed);
            Sonic.PlayAnimation("Run");
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeedIncrement = 0.5f;
            ObstacleManager = new ObstacleManager(GameSpeed);
            EnvironmentManager ??= new EnvironmentManager(GameSpeed);

            // Initialize UI elements
            RetryBtn = new MyButton("assets/images/GameScreen/retry_btn.png", 560, 318);
            MainMenuBtn = new MyButton("assets/images/GameScreen/main_menu_btn.png", 510, 368);
            ChooseArrow = SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png");
            GameOverBitmap = SplashKit.LoadBitmap("game_over", "assets/images/GameScreen/game_over.png");

            // Set initial state
            _currentState = new OpeningSceneState(this);
        }

        public void Update()
        {
            EnvironmentManager!.Update();
            Sonic!.Update();
            _currentState!.Update();
        }

        public void Draw()
        {
            EnvironmentManager!.Draw();
            Sonic!.Draw();
            ObstacleManager!.Draw();
            DrawScore();
            _currentState!.Draw();
        }

        public void SetState(IGameState newState)
        {
            _currentState = newState;
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            GameSpeed = gameSpeed;
            Sonic!.UpdateGameSpeed(gameSpeed);
            EnvironmentManager!.UpdateGameSpeed(gameSpeed);
            ObstacleManager!.UpdateGameSpeed(gameSpeed);
        }

        private void DrawScore()
        {
            string scoreStr = Math.Floor(Score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 24, 975, 30);
        }

        public void ExitState()
        {
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}