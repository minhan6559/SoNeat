using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;
using SoNeat.src.NEAT;
using SoNeat.src.UI.MainMenu;

namespace SoNeat.src.UI.TrainScreen
{
    public class TrainScreenState : GameScreenBase
    {
        private Population? _population;
        private Neat? _neat;
        private NetworkDrawer? _networkDrawer;
        private string _modelName = "Enter Model Name";
        private string _errorMessage = "";
        private string _successMessage = "";
        private bool _isFastForward = false;

        public Population? Population => _population;
        public string SuccessMessage
        {
            set => _successMessage = value;
        }

        public TrainScreenState(EnvironmentManager? environmentManager = null)
            : base(environmentManager) { }

        public override void EnterState()
        {
            string[] inputLabels = { "Sonic Y Position", "Distance To Next Enemy",
                "Next Enemy Width", "Next Enemy Height",
                "Bat Y Position", "Game Speed" };
            string[] outputLabels = { "Jump", "Duck" };

            GameSpeed = 10;
            _population = new Population(500);
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeedIncrement = 0.5f;
            ObstacleManager = new ObstacleManager(GameSpeed);
            EnvironmentManager ??= new EnvironmentManager(GameSpeed);

            InitializeButtons();
            InitializeUiBitmaps();

            _networkDrawer = new NetworkDrawer(inputLabels, outputLabels, 220, 10, 660, 320);

            SetState(new LoadModelState(this));
        }

        private void InitializeButtons()
        {
            Buttons = new Dictionary<string, MyButton>
            {
                { "MainMenuButton", new MyButton("assets/images/TrainScreen/train_main_menu_btn.png", 525, 403) },
                { "ChooseModelButton", new MyButton("assets/images/TrainScreen/choose_model_btn.png", 491, 314)},
                { "SaveModelButton", new MyButton("assets/images/TrainScreen/save_model.png", 522, 315)},
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

        public override void Update()
        {
            EnvironmentManager!.Update();
            CurrentState!.Update();
        }

        public override void Draw()
        {
            EnvironmentManager!.Draw();
            ObstacleManager!.Draw();
            CurrentState!.Draw();
        }

        public override void UpdateGameSpeed(float gameSpeed)
        {
            base.UpdateGameSpeed(gameSpeed);
            _population!.UpdateGameSpeed(gameSpeed);
        }

        public void UpdateFrameRate()
        {
            ScreenManager.FrameRate = _isFastForward ? 600 : 60;
        }

        public void Reset()
        {
            _population!.Reset();
            _neat!.Evolve();
            _population.LinkBrains(_neat);
            ObstacleManager!.Reset();
            Score = 0;
            LastScoreMilestone = 0;
            GameSpeed = 10;
            SetState(new TrainingState(this));
            UpdateGameSpeed(GameSpeed);
        }

        public void ToggleFastForward()
        {
            _isFastForward = !_isFastForward;
        }

        public void DrawTrainingInfo()
        {
            string scoreStr = Math.Floor(Score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 20, 973, 27);
            SplashKit.DrawText($"ALIVE:{_population!.Alives}", Color.Black, "MainFont", 20, 973, 63);
            SplashKit.DrawText($"GEN:{_neat!.Generation}", Color.Black, "MainFont", 20, 1013, 99);
        }

        public void DrawKeyboardShorcut()
        {
            SplashKit.DrawText("F-Fast Forward " + (_isFastForward ? "OFF" : "ON"), Color.Black, "MainFont", 15, 948, 145);
            SplashKit.DrawText("ESC-Pause", Color.Black, "MainFont", 15, 1015, 175);
        }

        public void DrawError()
        {
            if (_errorMessage != "")
            {
                SplashKit.DrawText(_errorMessage, SplashKit.RGBColor(194, 0, 0), "MainFont", 12, 535, 239);
            }
        }

        public void DrawSuccess()
        {
            if (_successMessage != "")
            {
                SplashKit.DrawText(_successMessage, SplashKit.RGBColor(0, 79, 172), "MainFont", 12, 487, 239);
            }
        }

        public bool CheckValidModelName()
        {
            if (File.Exists(Utility.NormalizePath($"save_contents/{_modelName}.json")))
            {
                _errorMessage = "";
                return true;
            }

            SplashKit.PlaySoundEffect("IncorrectSoundEffect", 0.5f);
            _errorMessage = "Model not found";
            return false;
        }

        public void GetModelNameFromTextBox()
        {
            _modelName = SplashKit.TextBox(_modelName, new Rectangle() { X = 491, Y = 264, Width = 263, Height = 32 });
        }

        public void LoadNeatModel()
        {
            _neat = Neat.DeserializeFromJson($"save_contents/{_modelName}.json");
            _population!.LinkBrains(_neat!);
            _modelName = "Enter Model Name";
        }

        public void SaveNeatModel()
        {
            _neat!.SerializeToJson($"save_contents/{_modelName}.json");
            _successMessage = "Model saved successfully";
        }

        public void InitializeNeatModel()
        {
            Neat.NextConnectionNum = 1000;
            _neat = new Neat(6, 2, 500);
            _population!.LinkBrains(_neat);
        }

        public bool IsAllDead()
        {
            return _population!.Alives <= 0;
        }

        public void DrawBestNetwork()
        {
            _networkDrawer!.Draw(_neat!.BestAgent.Genome);
        }

        public override void ExitState()
        {
            Console.WriteLine("Exiting Train Screen State");
        }

        public void PlayClickSound()
        {
            SplashKit.PlaySoundEffect("ClickSoundEffect", 0.2f);
        }
    }
}