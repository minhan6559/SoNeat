using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.GameLogic;

namespace SoNeat.src.Screen
{
    public class GameScreenState : IScreenState
    {
        // Game Elements
        private Ground? _ground;
        private Sonic? _sonic;
        private List<Obstacle>? _obstacles;
        private List<Cloud>? _clouds;

        // Game Logic
        private float _gameSpeed;
        private double _nextObstacleInterval;
        private double _nextCloudInterval;
        private static readonly SplashKitSDK.Timer _obstacleTimer = new("Obstacle Timer");
        private static readonly SplashKitSDK.Timer _cloudTimer = new("Cloud Timer");
        private static readonly Random _random = new Random();

        public void EnterState()
        {
            _gameSpeed = 10;
            _ground = new Ground(0, 634, _gameSpeed);
            _sonic = new Sonic(52, 509, _ground.Y, _gameSpeed);
            _obstacles = new List<Obstacle>();
            _clouds = new List<Cloud>();

            _obstacleTimer.Start();
            _nextObstacleInterval = 1000;

            _cloudTimer.Start();
            _nextCloudInterval = 500;
        }

        public void Update()
        {
            // Handle game logic, player input, and updates
            _sonic!.Update();
            _ground!.Update();

            // Loop Clouds
            for (int i = 0; i < _clouds!.Count; i++)
            {
                _clouds[i].Update();

                if (_clouds[i].IsOffScreen())
                {
                    _clouds.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < _obstacles!.Count; i++)
            {
                _obstacles[i].Update();

                if (_obstacles[i].IsColliding(_sonic))
                {
                    // ScreenManager.Instance.SetState(new GameScreenState());
                    Console.WriteLine("Game Over");
                }

                if (_obstacles[i].IsOffScreen())
                {
                    _obstacles.RemoveAt(i);
                    i--;
                }
            }

            // Check if it's time to create a new obstacle
            if (_obstacleTimer.Ticks > _nextObstacleInterval)
            {
                _obstacles.Add(ObstacleFactory.CreateObstacle(_gameSpeed));
                _obstacleTimer.Reset();
                SetNextObstacleInterval(); // Set a new random interval for the next obstacle
            }

            // Check if it's time to create a new cloud
            if (_cloudTimer.Ticks > _nextCloudInterval)
            {
                AddCloud();
                _cloudTimer.Reset();
                _nextCloudInterval = _random.NextDouble() * 1000 + 2000; // Set a new random interval for the next cloud
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

        private void AddCloud()
        {
            float randomY = _random.Next(55, 260);
            float randomSpeed = (float)(_random.NextDouble() * 1.0f + 3.0f);

            _clouds!.Add(new Cloud(SplashKit.ScreenWidth(), randomY, randomSpeed));
        }

        public void Draw()
        {
            // Draw the game elements to the screen
            foreach (Cloud cloud in _clouds!)
            {
                cloud.Draw();
            }

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