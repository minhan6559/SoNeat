using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;
using SoNeat.src.NEAT;
using SoNeat.src.UI.MainMenu;

namespace SoNeat.src.UI.TrainScreen
{
    public class TrainScreenState : IScreenState
    {
        private ITrainState? _currentState;
        public Population? Population { get; private set; }
        public Neat? Neat { get; set; }
        public ObstacleManager? ObstacleManager { get; private set; }
        public EnvironmentManager? EnvironmentManager { get; set; }
        public double Score { get; set; }
        public double LastScoreMilestone { get; set; }
        public float GameSpeed { get; set; }
        public float GameSpeedIncrement { get; set; }
        public Dictionary<string, MyButton>? Buttons { get; private set; }
        public Dictionary<string, Bitmap>? UiBitmaps { get; private set; }
        public NetworkDrawer? NetworkDrawer { get; private set; }
        public string ModelName { get; set; } = "Enter Model Name Here";
        public string ErrorMessage { get; set; } = "";
        public string SuccessMessage { get; set; } = "";
        public bool IsFastForward { get; set; } = false;

        public void EnterState()
        {
            string[] inputLabels = { "Sonic Y Position", "Distance To Next Enemy",
                "Next Enemy Width", "Next Enemy Height",
                "Bat Y Position", "Game Speed" };
            string[] outputLabels = { "Jump", "Duck" };

            GameSpeed = 10;
            Population = new Population(500);
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeedIncrement = 0.5f;
            ObstacleManager = new ObstacleManager(GameSpeed);
            EnvironmentManager ??= new EnvironmentManager(GameSpeed);

            InitializeButtons();
            InitializeUiBitmaps();

            NetworkDrawer = new NetworkDrawer(inputLabels, outputLabels, 220, 10, 660, 320);

            SetState(new LoadModelState(this));
        }

        private void InitializeButtons()
        {
            Buttons = new Dictionary<string, MyButton>
            {
                { "MainMenuButton", new MyButton("assets/images/TrainScreen/main_menu_btn.png", 526, 403) },
                { "ChooseModelButton", new MyButton("assets/images/TrainScreen/choose_model_btn.png", 504, 317)},
                { "SaveModelButton", new MyButton("assets/images/TrainScreen/save_model.png", 527, 317)},
                { "ResumeButton", new MyButton("assets/images/TrainScreen/resume_btn.png", 557, 358)},
                { "RetrainButton", new MyButton("assets/images/TrainScreen/retrain_btn.png", 547, 358)}
            };
        }

        private void InitializeUiBitmaps()
        {
            UiBitmaps = new Dictionary<string, Bitmap>
            {
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png")},
                { "LoadModelTitle", SplashKit.LoadBitmap("load_model_title", "assets/images/TrainScreen/load_model_title.png")},
                { "PausedTitle", SplashKit.LoadBitmap("paused_title", "assets/images/TrainScreen/pause_title.png")}
            };
        }

        public void Update()
        {
            EnvironmentManager!.Update();
            _currentState!.Update();
        }

        public void Draw()
        {
            EnvironmentManager!.Draw();
            ObstacleManager!.Draw();
            _currentState!.Draw();
        }

        public void SetState(ITrainState state)
        {
            _currentState = state;
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            Population!.UpdateGameSpeed(gameSpeed);
            EnvironmentManager!.UpdateGameSpeed(gameSpeed);
            ObstacleManager!.UpdateGameSpeed(gameSpeed);
        }

        public void Reset()
        {
            Population!.Reset();
            Neat!.Evolve();
            Population.LinkBrains(Neat);
            ObstacleManager!.Reset();
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeed = 10;
            SetState(new TrainingState(this));
            UpdateGameSpeed(GameSpeed);
        }

        public void DrawTrainingInfo()
        {
            string scoreStr = Math.Floor(Score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 24, 975, 30);
            SplashKit.DrawText($"ALIVE:{Population!.Alives}", Color.Black, "MainFont", 24, 975, 65);
            SplashKit.DrawText($"GEN:{Neat!.Generation}", Color.Black, "MainFont", 24, 1023, 101);
        }

        public void DrawError()
        {
            if (ErrorMessage != "")
            {
                SplashKit.DrawText(ErrorMessage, SplashKit.RGBColor(194, 0, 0), "MainFont", 12, 535, 239);
            }
        }

        public void DrawSuccess()
        {
            if (SuccessMessage != "")
            {
                SplashKit.DrawText(SuccessMessage, SplashKit.RGBColor(0, 79, 172), "MainFont", 12, 487, 239);
            }
        }

        public bool CheckValidModelName()
        {
            if (File.Exists(Utility.NormalizePath($"saved_models/{ModelName}.json")))
            {
                ErrorMessage = "";
                return true;
            }
            else
            {
                ErrorMessage = "Model not found!";
                return false;
            }
        }

        public void GetModelNameFromTextBox()
        {
            ModelName = SplashKit.TextBox(ModelName, new Rectangle() { X = 465, Y = 264, Width = 320, Height = 32 });
        }

        public void ExitState()
        {
            Console.WriteLine("Exiting Train Screen State");
        }
    }
}