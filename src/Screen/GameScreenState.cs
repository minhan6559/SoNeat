using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.GameLogic;
using SoNeat.src.Utils;

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
        private double _score;
        private double _lastScoreMilestone;
        private float _gameSpeed;
        private float _gameSpeedIncrement;
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

            SplashKit.LoadFont("PressStart2P", Utility.NormalizePath("assets/fonts/PressStart2P.ttf"));

            _score = 0;
            _lastScoreMilestone = 0;
            _gameSpeedIncrement = 0.5f;

            _obstacleTimer.Start();
            _nextObstacleInterval = 1000;

            _cloudTimer.Start();
            _nextCloudInterval = 500;
        }

        public void Update()
        {
            _score += _gameSpeed / 60;
            if (Math.Floor(_score) >= _lastScoreMilestone + 100)
            {
                _lastScoreMilestone = Math.Floor(_score);
                UpdateGameSpeed(_gameSpeed + _gameSpeedIncrement);
            }

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
            _sonic!.UpdateGameSpeed(gameSpeed);
            _ground!.UpdateGameSpeed(gameSpeed);
            foreach (Obstacle obstacle in _obstacles!)
            {
                obstacle.UpdateGameSpeed(gameSpeed);
            }
        }

        private void SetNextObstacleInterval()
        {
            // Base interval is reduced as the game speed increases
            double baseInterval = Math.Max(9000 / _gameSpeed, 600);

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

            DrawScore();
        }

        public void DrawScore()
        {
            string scoreStr = Math.Floor(_score).ToString();

            // Padding the score with zeros to fit 5 digits
            while (scoreStr.Length < 5)
            {
                scoreStr = "0" + scoreStr;
            }

            SplashKit.DrawText($"SCORE:{scoreStr}", Color.Black, "PressStart2P", 24, 925, 30);
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}