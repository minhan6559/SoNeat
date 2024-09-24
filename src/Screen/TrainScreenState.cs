using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;
using SoNeat.src.NEAT;

namespace SoNeat.src.Screen
{
    public enum TrainScreenStateType
    {
        Training,
        Saving
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
        private MyButton? _mainMenuBtn;
        private Bitmap? _chooseArrow;
        private NetworkDrawer? _networkDrawer;

        public void EnterState()
        {
            string[] inputLabels = ["Sonic Y Position", "Distance To Next Enemy",
                                    "Next Enemy Width", "Next Enemy Height",
                                    "Bat Y Position", "Game Speed"];
            string[] outputLabels = ["Jump", "Duck"];
            _gameSpeed = 10;

            _population = new Population(500);
            _neat = new Neat(inputLabels.Length, outputLabels.Length, 500);
            _population.LinkBrains(_neat);

            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeedIncrement = 0.5f;

            _obstacleManager = new ObstacleManager(_gameSpeed);
            _obstacleManager.StartTimer();

            if (_environmentManager == null)
                _environmentManager = new EnvironmentManager(_gameSpeed);

            _gameStateType = TrainScreenStateType.Training;
            UpdateGameSpeed(_gameSpeed);

            _mainMenuBtn = new MyButton("assets/images/GameScreen/main_menu_btn.png", 510, 368);
            _chooseArrow = SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png");

            _networkDrawer = new NetworkDrawer(inputLabels, outputLabels, 220, 10, 660, 320);
        }

        public void LoadEnvironment(EnvironmentManager oldEnvironment)
        {
            _environmentManager = oldEnvironment;
        }

        public void Update()
        {
            switch (_gameStateType)
            {
                case TrainScreenStateType.Training:
                    _environmentManager!.Update();
                    _score += _gameSpeed / 60;
                    if (Math.Floor(_score) >= _lastScoreMilestone + 100)
                    {
                        _lastScoreMilestone = Math.Floor(_score);
                        UpdateGameSpeed(_gameSpeed + _gameSpeedIncrement);
                    }

                    _obstacleManager!.Update(_population!, _score);
                    _population!.Update(_obstacleManager!.Obstacles);


                    if (_population!.Alives <= 0)
                    {
                        Reset();
                    }
                    break;
                default:
                    break;
            }
        }

        private void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;
            _population!.UpdateGameSpeed(gameSpeed);

            _environmentManager!.UpdateGameSpeed(gameSpeed);
            _obstacleManager!.UpdateGameSpeed(gameSpeed);
        }

        public void Reset()
        {
            _population!.Reset();
            _neat!.Evolve();
            // _neat.PrintSpecies();
            _population!.LinkBrains(_neat);

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

            DrawTrainingInfo();
            _networkDrawer!.Draw(_population!.BestBrain);

            switch (_gameStateType)
            {
                case TrainScreenStateType.Training:
                    _population!.Draw();
                    break;
                case TrainScreenStateType.Saving:

                    _mainMenuBtn!.Draw();
                    if (_mainMenuBtn.IsHovered())
                    {
                        _chooseArrow!.Draw(_mainMenuBtn.X - 40, _mainMenuBtn.Y);
                    }
                    break;
            }
        }

        private void DrawTrainingInfo()
        {
            string scoreStr = Math.Floor(_score).ToString().PadLeft(5, '0');
            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 24, 975, 30);

            SplashKit.DrawText($"ALIVE:{_population!.Alives}", Color.Black, "MainFont", 24, 975, 65);

            SplashKit.DrawText($"GEN:{_population!.Generation}", Color.Black, "MainFont", 24, 1023, 101);
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}