using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;

namespace SoNeat.src.UI
{
    // Base class for game screens
    public abstract class GameScreenBase : IScreenState
    {
        private ISubScreenState? _currentState; // Current state of the game screen
        private ObstacleManager? _obstacleManager; // Obstacle manager for the game screen
        private EnvironmentManager? _environmentManager; // Environment manager for the game screen
        private double _score, _lastScoreMilestone;
        private float _gameSpeed, _gameSpeedIncrement;
        private Dictionary<string, MyButton>? _buttons;
        private Dictionary<string, Bitmap>? _uiBitmaps;

        protected ISubScreenState? CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }

        protected double Score
        {
            get => _score;
            set => _score = value;
        }

        protected double LastScoreMilestone
        {
            get => _lastScoreMilestone;
            set => _lastScoreMilestone = value;
        }

        protected float GameSpeed
        {
            get => _gameSpeed;
            set => _gameSpeed = value;
        }

        protected float GameSpeedIncrement
        {
            get => _gameSpeedIncrement;
            set => _gameSpeedIncrement = value;
        }

        public ObstacleManager? ObstacleManager
        {
            get => _obstacleManager;
            protected set => _obstacleManager = value;
        }

        public EnvironmentManager? EnvironmentManager
        {
            get => _environmentManager;
            protected set => _environmentManager = value;
        }

        public Dictionary<string, MyButton>? Buttons
        {
            get => _buttons;
            protected set => _buttons = value;
        }

        public Dictionary<string, Bitmap>? UiBitmaps
        {
            get => _uiBitmaps;
            protected set => _uiBitmaps = value;
        }

        protected GameScreenBase(EnvironmentManager? environmentManager = null)
        {
            _environmentManager = environmentManager;
        }

        public abstract void EnterState();
        public abstract void Update();
        public abstract void Draw();
        public abstract void ExitState();

        public virtual void SetState(ISubScreenState state)
        {
            _currentState = state;
        }

        public virtual void UpdateGameSpeed(float gameSpeed)
        {
            _environmentManager!.UpdateGameSpeed(gameSpeed);
            _obstacleManager!.UpdateGameSpeed(gameSpeed);
        }

        protected virtual void IncreaseGameSpeed()
        {
            _gameSpeed += _gameSpeedIncrement;
            UpdateGameSpeed(_gameSpeed);
        }

        public virtual void CheckUpdateGameSpeed()
        {
            if (Math.Floor(_score) >= _lastScoreMilestone + 100)
            {
                _lastScoreMilestone = Math.Floor(_score);
                IncreaseGameSpeed();
            }
        }

        public virtual void ResumeGameSpeed()
        {
            UpdateGameSpeed(_gameSpeed);
        }

        public virtual void UpdateScore()
        {
            _score += _gameSpeed / 60;
        }
    }
}