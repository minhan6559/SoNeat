using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;

namespace SoNeat.src.Screen
{
    enum GameScreenStateType
    {
        OpeningScene,
        Idle,
        Playing,
        GameOver
    }

    public class GameScreenState : IScreenState
    {
        // Game Elements
        private Sonic? _sonic;
        private ObstacleManager? _obstacleManager;
        private EnvironmentManager? _environmentManager;

        // Game Logic
        private double _score;
        private double _lastScoreMilestone;
        private float _gameSpeed;
        private float _gameSpeedIncrement;
        private GameScreenStateType _gameStateType;

        // UI
        private MyButton? _retryBtn;
        private MyButton? _mainMenuBtn;
        private Bitmap? _chooseArrow;
        private Bitmap? _gameOverBitmap;

        public void EnterState()
        {
            _gameSpeed = 10;

            _sonic = new Sonic(-110, 509, 634, _gameSpeed);
            _sonic.Sprite.Play("Run");

            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeedIncrement = 0.5f;

            _obstacleManager = new ObstacleManager(_gameSpeed);
            if (_environmentManager == null)
                _environmentManager = new EnvironmentManager(_gameSpeed);

            _gameStateType = GameScreenStateType.OpeningScene;

            _retryBtn = new MyButton("assets/images/GameScreen/retry_btn.png", 560, 318);
            _mainMenuBtn = new MyButton("assets/images/GameScreen/main_menu_btn.png", 510, 368);
            _chooseArrow = SplashKit.LoadBitmap("choose_arrow", "assets/images/choose_arrow.png");
            _gameOverBitmap = SplashKit.LoadBitmap("game_over", "assets/images/GameScreen/game_over.png");
        }

        public void LoadEnvironment(EnvironmentManager oldEnvironment)
        {
            _environmentManager = oldEnvironment;
        }

        public void Update()
        {
            if (_gameStateType != GameScreenStateType.GameOver)
            {
                _environmentManager!.Update();
                _sonic!.Update();
            }

            switch (_gameStateType)
            {
                case GameScreenStateType.OpeningScene:
                    _sonic!.X += 5;
                    if (_sonic.X >= 52)
                    {
                        _sonic.X = 52;
                        _sonic.Sprite.Play("Idle");
                        _gameStateType = GameScreenStateType.Idle;
                    }
                    break;
                case GameScreenStateType.Idle:
                    if (SplashKit.KeyTyped(KeyCode.SpaceKey))
                    {
                        _sonic!.IsIdle = false;
                        UpdateGameSpeed(_gameSpeed);
                        _gameStateType = GameScreenStateType.Playing;
                        _obstacleManager!.StartTimer();
                    }
                    break;
                case GameScreenStateType.Playing:
                    _sonic!.HandleInput();
                    _score += _gameSpeed / 60;
                    if (Math.Floor(_score) >= _lastScoreMilestone + 100)
                    {
                        _lastScoreMilestone = Math.Floor(_score);
                        UpdateGameSpeed(_gameSpeed + _gameSpeedIncrement);
                    }

                    _obstacleManager!.Update(_sonic);
                    if (_sonic.IsDead)
                    {
                        _sonic.Sprite.Play("Dead");
                        _gameStateType = GameScreenStateType.GameOver;
                        UpdateGameSpeed(0);
                    }
                    break;
                case GameScreenStateType.GameOver:
                    if (_retryBtn!.IsClicked())
                    {
                        GameScreenState gameScreen = new GameScreenState();
                        gameScreen.LoadEnvironment(_environmentManager!);
                        ScreenManager.Instance.SetState(gameScreen);
                    }

                    if (_mainMenuBtn!.IsClicked())
                    {
                        ScreenManager.Instance.SetState(new MainMenuState());
                    }
                    break;
            }
        }

        private void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;
            _sonic!.UpdateGameSpeed(gameSpeed);

            _environmentManager!.UpdateGameSpeed(gameSpeed);
            _obstacleManager!.UpdateGameSpeed(gameSpeed);
        }

        public void Draw()
        {
            _environmentManager!.Draw();
            _sonic!.Draw();

            _obstacleManager!.Draw();

            DrawScore();

            switch (_gameStateType)
            {
                case GameScreenStateType.OpeningScene:
                    break;
                case GameScreenStateType.Idle:
                    SplashKit.DrawText("Press SPACE to start", Color.Black, "MainFont", 24, 400, 300);
                    break;
                case GameScreenStateType.Playing:
                    break;
                case GameScreenStateType.GameOver:
                    SplashKit.DrawBitmap(_gameOverBitmap!, 305, 151);

                    _retryBtn!.Draw();
                    if (_retryBtn.IsHovered())
                    {
                        _chooseArrow!.Draw(_retryBtn.X - 40, _retryBtn.Y);
                    }

                    _mainMenuBtn!.Draw();
                    if (_mainMenuBtn.IsHovered())
                    {
                        _chooseArrow!.Draw(_mainMenuBtn.X - 40, _mainMenuBtn.Y);
                    }
                    break;
            }
        }

        public void DrawScore()
        {
            string scoreStr = Math.Floor(_score).ToString().PadLeft(5, '0');

            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "MainFont", 24, 975, 30);
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}