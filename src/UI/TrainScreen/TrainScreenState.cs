using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;
using SoNeat.src.NEAT;
using SoNeat.src.UI.MainMenu;

namespace SoNeat.src.UI.TrainScreen
{
    public class TrainScreenState : IScreenState
    {
        private ISubScreenState? _currentState;
        private Population? _population;
        private Neat? _neat;
        private ObstacleManager? _obstacleManager;
        private EnvironmentManager? _environmentManager;
        private double _score, _lastScoreMilestone;
        private float _gameSpeed, _gameSpeedIncrement;
        private Dictionary<string, MyButton>? _buttons;
        private Dictionary<string, Bitmap>? _uiBitmaps;
        private NetworkDrawer? _networkDrawer;
        private string _modelName = "Enter Model Name";
        private string _errorMessage = "", _successMessage = "";
        private bool _isFastForward = false;

        public Population? Population => _population;

        public ObstacleManager? ObstacleManager => _obstacleManager;

        public EnvironmentManager? EnvironmentManager => _environmentManager;

        public Dictionary<string, MyButton>? Buttons => _buttons;

        public Dictionary<string, Bitmap>? UiBitmaps => _uiBitmaps;

        public string SuccessMessage
        {
            get => _successMessage;
            set => _successMessage = value;
        }

        public TrainScreenState(EnvironmentManager? environmentManager = null)
        {
            _environmentManager = environmentManager;
        }

        public void EnterState()
        {
            string[] inputLabels = { "Sonic Y Position", "Distance To Next Enemy",
                "Next Enemy Width", "Next Enemy Height",
                "Bat Y Position", "Game Speed" };
            string[] outputLabels = { "Jump", "Duck" };

            _gameSpeed = 10;
            _population = new Population(500);
            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeedIncrement = 0.5f;
            _obstacleManager = new ObstacleManager(_gameSpeed);
            _environmentManager ??= new EnvironmentManager(_gameSpeed);

            InitializeButtons();
            InitializeUiBitmaps();

            _networkDrawer = new NetworkDrawer(inputLabels, outputLabels, 220, 10, 660, 320);

            SetState(new LoadModelState(this));
        }

        private void InitializeButtons()
        {
            _buttons = new Dictionary<string, MyButton>
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
            _uiBitmaps = new Dictionary<string, Bitmap>
            {
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png")},
                { "LoadModelTitle", SplashKit.LoadBitmap("load_model_title", "assets/images/TrainScreen/load_model_title.png")},
                { "PausedTitle", SplashKit.LoadBitmap("paused_title", "assets/images/TrainScreen/pause_title.png")}
            };
        }

        public void Update()
        {
            _environmentManager!.Update();
            _currentState!.Update();
        }

        public void Draw()
        {
            _environmentManager!.Draw();
            _obstacleManager!.Draw();
            _currentState!.Draw();
        }

        public void SetState(ISubScreenState state)
        {
            _currentState = state;
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            _population!.UpdateGameSpeed(gameSpeed);
            _environmentManager!.UpdateGameSpeed(gameSpeed);
            _obstacleManager!.UpdateGameSpeed(gameSpeed);
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

        public void UpdateFrameRate()
        {
            ScreenManager.FrameRate = _isFastForward ? 600 : 60;
        }

        public void Reset()
        {
            _population!.Reset();
            _neat!.Evolve();
            _population.LinkBrains(_neat);
            _obstacleManager!.Reset();
            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeed = 10;
            SetState(new TrainingState(this));
            UpdateGameSpeed(_gameSpeed);
        }

        public void ToggleFastForward()
        {
            _isFastForward = !_isFastForward;
        }

        public void DrawTrainingInfo()
        {
            string scoreStr = Math.Floor(_score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 20, 973, 27);
            SplashKit.DrawText($"ALIVE:{_population!.Alives}", Color.Black, "MainFont", 20, 973, 63);
            SplashKit.DrawText($"GEN:{_neat!.Generation}", Color.Black, "MainFont", 20, 1013, 99);
        }

        public void DrawKeyboardShorcut()
        {
            SplashKit.DrawText("F-Toggle Fast Forward", Color.Black, "MainFont", 15, 925, 145);
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
            else
            {
                _errorMessage = "Model not found";
                return false;
            }
        }

        public void GetModelNameFromTextBox()
        {
            _modelName = SplashKit.TextBox(_modelName, new Rectangle() { X = 491, Y = 264, Width = 263, Height = 32 });
        }

        public void LoadNeatModel()
        {
            _neat = Neat.DeserializeFromJson($"save_contents/{_modelName}.json");
            Console.WriteLine(Neat.NextConnectionNum);
            _population!.LinkBrains(_neat!);
            _modelName = "Enter Model Name";
        }

        public void SaveNeatModel()
        {
            _neat!.SerializeToJson($"save_contents/{_modelName}.json");
            Console.WriteLine(Neat.NextConnectionNum);
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

        public void ExitState()
        {
            Console.WriteLine("Exiting Train Screen State");
        }
    }
}