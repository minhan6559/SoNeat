using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;
using SoNeat.src.NEAT;
using SoNeat.src.UI.MainMenu;

namespace SoNeat.src.UI.TrainScreen
{
    public enum TrainScreenStateType
    {
        LoadModel,
        Training,
        Paused
    }

    public class TrainScreenState : IScreenState
    {
        // Game Elements
        private Population? _population;
        private Neat? _neat;
        private ObstacleManager? _obstacleManager;
        private EnvironmentManager? _environmentManager;

        // Game Logic
        private double _score;
        private double _lastScoreMilestone;
        private float _gameSpeed;
        private float _gameSpeedIncrement;
        private TrainScreenStateType _gameStateType;

        // UI
        private Dictionary<string, MyButton>? _buttons;
        private Dictionary<string, Bitmap>? _uiBitmaps;
        private NetworkDrawer? _networkDrawer;
        private string _modelName = "Enter Model Name Here";
        private string _errorMessage = "";
        private string _successMessage = "";

        public EnvironmentManager? EnvironmentManager
        {
            get => _environmentManager;
            set => _environmentManager = value;
        }

        public void EnterState()
        {
            string[] inputLabels = ["Sonic Y Position", "Distance To Next Enemy",
                                    "Next Enemy Width", "Next Enemy Height",
                                    "Bat Y Position", "Game Speed"];
            string[] outputLabels = ["Jump", "Duck"];
            _gameSpeed = 10;

            _population = new Population(500);
            // _neat = new Neat(inputLabels.Length, outputLabels.Length, 500);
            // _population.LinkBrains(_neat!);

            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeedIncrement = 0.5f;

            _obstacleManager = new ObstacleManager(_gameSpeed);

            _environmentManager ??= new EnvironmentManager(_gameSpeed);

            _gameStateType = TrainScreenStateType.LoadModel;
            UpdateGameSpeed(0);

            _buttons = new Dictionary<string, MyButton>
            {
                { "MainMenuButton", new MyButton("assets/images/TrainScreen/main_menu_btn.png", 526, 403) },
                { "ChooseModelButton", new MyButton("assets/images/TrainScreen/choose_model_btn.png", 504, 317)},
                { "SaveModelButton", new MyButton("assets/images/TrainScreen/save_model.png", 527, 317)},
                { "ResumeButton", new MyButton("assets/images/TrainScreen/resume_btn.png", 557, 358)},
                { "RetrainButton", new MyButton("assets/images/TrainScreen/retrain_btn.png", 547, 358)}
            };

            _uiBitmaps = new Dictionary<string, Bitmap>
            {
                { "ChooseArrow", SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png")},
                { "LoadModelTitle", SplashKit.LoadBitmap("load_model_title", "assets/images/TrainScreen/load_model_title.png")},
                { "PausedTitle", SplashKit.LoadBitmap("paused_title", "assets/images/TrainScreen/pause_title.png")}
            };

            _networkDrawer = new NetworkDrawer(inputLabels, outputLabels, 220, 10, 660, 320);
        }

        public void Update()
        {
            _environmentManager!.Update();
            switch (_gameStateType)
            {
                case TrainScreenStateType.LoadModel:
                    GetModelNameFromTextBox();
                    if (_buttons!["ChooseModelButton"].IsClicked() && CheckValidModelName())
                    {
                        UpdateGameSpeed(_gameSpeed);
                        _neat = Neat.DeserializeFromJson($"saved_models/{_modelName}.json");
                        _population!.LinkBrains(_neat!);
                        _gameStateType = TrainScreenStateType.Training;
                        _modelName = "Enter Model Name Here";
                    }

                    if (_buttons!["RetrainButton"].IsClicked())
                    {
                        UpdateGameSpeed(_gameSpeed);
                        Neat.NextConnectionNum = 1000;
                        _neat = new Neat(6, 2, 500);
                        _population!.LinkBrains(_neat!);
                        _gameStateType = TrainScreenStateType.Training;
                    }

                    if (_buttons!["MainMenuButton"].IsClicked())
                    {
                        MainMenuState mainMenuState = new MainMenuState();
                        mainMenuState.EnvironmentManager = _environmentManager;
                        ScreenManager.Instance.SetState(mainMenuState);
                    }
                    break;
                case TrainScreenStateType.Training:
                    _score += _gameSpeed / 60;
                    if (Math.Floor(_score) >= _lastScoreMilestone + 100)
                    {
                        _lastScoreMilestone = Math.Floor(_score);
                        _gameSpeed += _gameSpeedIncrement;
                        UpdateGameSpeed(_gameSpeed);
                    }

                    _obstacleManager!.Update(_population!, _score);
                    _population!.Update(_obstacleManager!.Obstacles);

                    if (_population!.Alives <= 0)
                        Reset();

                    if (SplashKit.KeyTyped(KeyCode.EscapeKey))
                    {
                        UpdateGameSpeed(0);
                        _gameStateType = TrainScreenStateType.Paused;
                    }
                    break;
                case TrainScreenStateType.Paused:
                    GetModelNameFromTextBox();
                    if (_buttons!["ResumeButton"].IsClicked())
                    {
                        _successMessage = "";
                        _gameStateType = TrainScreenStateType.Training;
                        UpdateGameSpeed(_gameSpeed);
                    }

                    if (_buttons!["MainMenuButton"].IsClicked())
                    {
                        MainMenuState mainMenuState = new MainMenuState();
                        mainMenuState.EnvironmentManager = _environmentManager;
                        ScreenManager.Instance.SetState(mainMenuState);
                    }

                    if (_buttons!["SaveModelButton"].IsClicked())
                    {
                        _neat!.SerializeToJson($"saved_models/{_modelName}.json");
                        _successMessage = "Model saved successfully";
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateGameSpeed(float gameSpeed)
        {
            _population!.UpdateGameSpeed(gameSpeed);

            _environmentManager!.UpdateGameSpeed(gameSpeed);
            _obstacleManager!.UpdateGameSpeed(gameSpeed);
        }

        public void Reset()
        {
            _population!.Reset();
            _neat!.Evolve();
            _population!.LinkBrains(_neat!);

            _obstacleManager!.Reset();
            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeed = 10;
            _gameStateType = TrainScreenStateType.Training;
            UpdateGameSpeed(_gameSpeed);
        }

        public void Draw()
        {
            _environmentManager!.Draw();
            _obstacleManager!.Draw();

            switch (_gameStateType)
            {
                case TrainScreenStateType.LoadModel:
                    _uiBitmaps!["LoadModelTitle"].Draw(278, 145);
                    DrawError();
                    // Loop through these buttons: ChooseModelButton, MainMenuButton, RetrainButton
                    foreach (string buttonName in new string[] { "ChooseModelButton", "MainMenuButton", "RetrainButton" })
                    {
                        MyButton button = _buttons![buttonName];
                        button.Draw();
                        if (button.IsHovered())
                        {
                            SplashKit.DrawBitmap(_uiBitmaps!["ChooseArrow"], button.X - 40, button.Y);
                        }
                    }
                    SplashKit.DrawInterface();
                    break;
                case TrainScreenStateType.Training:
                    DrawTrainingInfo();
                    _population!.Draw();
                    _networkDrawer!.Draw(_neat!.BestAgent.Genome);
                    break;
                case TrainScreenStateType.Paused:
                    _uiBitmaps!["PausedTitle"].Draw(410, 145);
                    DrawSuccess();
                    _population!.Draw();
                    // Loop through these buttons: ResumeButton, MainMenuButton, SaveModelButton
                    foreach (string buttonName in new string[] { "ResumeButton", "MainMenuButton", "SaveModelButton" })
                    {
                        MyButton button = _buttons![buttonName];
                        button.Draw();
                        if (button.IsHovered())
                        {
                            SplashKit.DrawBitmap(_uiBitmaps!["ChooseArrow"], button.X - 40, button.Y);
                        }
                    }
                    SplashKit.DrawInterface();
                    break;
            }
        }

        private void DrawTrainingInfo()
        {
            string scoreStr = Math.Floor(_score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 24, 975, 30);

            SplashKit.DrawText($"ALIVE:{_population!.Alives}", Color.Black, "MainFont", 24, 975, 65);

            SplashKit.DrawText($"GEN:{_neat!.Generation}", Color.Black, "MainFont", 24, 1023, 101);
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
            // Check if the model name is existing in saved_models folder
            if (File.Exists(Utility.NormalizePath($"saved_models/{_modelName}.json")))
            {
                _errorMessage = "";
                return true;
            }
            else
            {
                _errorMessage = "Model not found!";
                return false;
            }
        }

        public void GetModelNameFromTextBox()
        {
            _modelName = SplashKit.TextBox(_modelName, new Rectangle() { X = 465, Y = 264, Width = 320, Height = 32 });
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}