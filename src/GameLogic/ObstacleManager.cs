using System;
using System.Collections.Generic;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public class ObstacleManager
    {
        private List<Obstacle> _obstacles;
        private readonly SplashKitSDK.Timer _obstacleTimer;
        private double _nextObstacleInterval;
        private float _gameSpeed;
        private int _countObstacles;
        private static readonly Random _random = new Random();

        public List<Obstacle> Obstacles => _obstacles;

        public ObstacleManager(float initialGameSpeed)
        {
            _obstacles = new List<Obstacle>();
            _obstacleTimer = new SplashKitSDK.Timer("Obstacle Timer");
            _gameSpeed = initialGameSpeed;
            _nextObstacleInterval = 1000;
            _countObstacles = 0;
        }

        public void Update(Sonic sonic)
        {
            for (int i = 0; i < _obstacles.Count; i++)
            {
                _obstacles[i].Update();

                if (!sonic.IsDead && _obstacles[i].IsColliding(sonic))
                {
                    // Handle collision
                    sonic.IsDead = true;
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
                if (_countObstacles < 3 && _random.NextDouble() < 0.75)
                {
                    _obstacles.Add(ObstacleFactory.CreateObstacle(_gameSpeed, ObstacleType.Bat));
                }
                else
                    _obstacles.Add(ObstacleFactory.CreateObstacle(_gameSpeed));
                _countObstacles++;
                _obstacleTimer.Reset();
                SetNextObstacleInterval();
            }
        }

        public void Update(Population population)
        {
            foreach (Obstacle obstacle in _obstacles)
            {
                obstacle.Update();
                obstacle.CheckPassedPlayer(population!.Data![0]);
            }

            for (int i = 0; i < population!.Data!.Length; i++)
            {
                if (population.Data[i].IsDead)
                {
                    continue;
                }

                for (int j = 0; j < Math.Min(2, _obstacles.Count); j++)
                {
                    if (_obstacles[j].IsColliding(population.Data[i]))
                    {
                        if (population.Data[i].IsJumping && _obstacles[j] is Bat)
                        {
                            population.Data[i].JumpOverBats++;
                        }
                        population.Data[i].IsDead = true;
                        population.Alives--;
                    }
                    else if (_obstacles[j].HasPassedPlayer && !_obstacles[j].AlreadyCheckedPass && _obstacles[j] is Bat)
                    {
                        if (population.Data[i].IsDucking)
                        {
                            population.Data[i].DuckUnderBats++;
                        }
                        // if (population.Data[i].IsJumping)
                        else
                        {
                            population.Data[i].JumpOverBats++;
                        }
                    }
                }
            }

            for (int i = 0; i < Math.Min(2, _obstacles.Count); i++)
            {
                if (_obstacles[i].HasPassedPlayer)
                {
                    _obstacles[i].AlreadyCheckedPass = true;
                }
            }

            for (int i = 0; i < _obstacles.Count; i++)
            {
                if (_obstacles[i].IsOffScreen())
                {
                    _obstacles.RemoveAt(i);
                    break;
                }
            }

            if (_obstacleTimer.Ticks > _nextObstacleInterval)
            {
                _obstacles.Add(ObstacleFactory.CreateObstacle(_gameSpeed));
                _obstacleTimer.Reset();
                SetNextObstacleInterval();
            }
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;
            foreach (Obstacle obstacle in _obstacles)
            {
                obstacle.UpdateGameSpeed(gameSpeed);
            }
        }

        public void StartTimer()
        {
            _obstacleTimer.Start();
        }

        private void SetNextObstacleInterval()
        {
            double baseInterval = Math.Max(9000 / _gameSpeed, 500);
            _nextObstacleInterval = _random.NextDouble() * baseInterval + baseInterval;
        }

        public void Draw()
        {
            foreach (Obstacle obstacle in _obstacles)
            {
                obstacle.Draw();
            }
        }

        public void Reset()
        {
            _obstacles.Clear();
            _obstacleTimer.Reset();
            _nextObstacleInterval = 1000;
            _countObstacles = 0;
        }
    }
}