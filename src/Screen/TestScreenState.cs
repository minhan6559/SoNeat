using System;
using System.Collections.Generic;
using System.Linq;

using SoNeat.src.GameLogic;
using SplashKitSDK;

namespace SoNeat.src.Screen
{
    public class TestScreenState : IScreenState
    {
        private Ground? _ground;
        private Sonic? _sonic;
        private float _gameSpeed;
        private List<Obstacle>? _obstacles;
        private double _timeSinceLastObstacle;
        private double _nextObstacleInterval;
        private static readonly SplashKitSDK.Timer _timer = new("GameTimer");
        private static readonly Random _random = new Random();

        public void EnterState()
        {
            _gameSpeed = 12;
            _ground = new Ground(0, 634, _gameSpeed);
            _sonic = new Sonic(52, 509, _ground.Y, _gameSpeed);
            _obstacles = new List<Obstacle>();
            _timer.Start();
            _timeSinceLastObstacle = 0;
            SetNextObstacleInterval();
        }

        public void Update()
        {
            // Handle game logic, player input, and updates
            _sonic!.Update();
            _ground!.Update();

            for (int i = 0; i < _obstacles!.Count; i++)
            {
                _obstacles[i].Update();

                if (_obstacles[i].IsColliding(_sonic))
                {
                    ScreenManager.Instance.SetState(new GameScreenState());
                }

                if (_obstacles[i].IsOffScreen())
                {
                    _obstacles.RemoveAt(i);
                    i--;
                }
            }

            _timeSinceLastObstacle += _timer.Ticks;
            _timer.Reset();

            // Check if it's time to create a new obstacle
            if (_timeSinceLastObstacle > _nextObstacleInterval)
            {
                _obstacles.Add(ObstacleFactory.CreateObstacle(_gameSpeed));
                _timeSinceLastObstacle = 0;
                SetNextObstacleInterval(); // Set a new random interval for the next obstacle
            }
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;
            _sonic!.GameSpeed = gameSpeed;
            _ground!.GameSpeed = gameSpeed;
            foreach (Obstacle obstacle in _obstacles!)
            {
                obstacle.GameSpeed = gameSpeed;
            }
        }

        private void SetNextObstacleInterval()
        {
            // Base interval is reduced as the game speed increases
            double baseInterval = Math.Max(2000 / _gameSpeed, 800);

            // Add some randomness to the interval
            _nextObstacleInterval = _random.NextDouble() * baseInterval + baseInterval;
        }

        public void Draw()
        {
            // Draw the game elements to the screen
            _sonic!.Draw();
            _ground!.Draw();
            foreach (Obstacle obstacle in _obstacles!)
            {
                obstacle.Draw();
            }
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}