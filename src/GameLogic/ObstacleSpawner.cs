using System;
using System.Collections.Generic;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public class ObstacleSpawner : RandomSpawnerBase
    {
        private int _countObstacles;

        public List<Obstacle> Obstacles => _gameObjects.OfType<Obstacle>().ToList();

        public ObstacleSpawner(float initialGameSpeed) : base(initialGameSpeed)
        {
            _nextSpawnInterval = 50;
            _countObstacles = 0;
        }

        public void Update(Sonic sonic)
        {
            base.Update();
            if (IsColliding(sonic))
            {
                sonic.IsDead = true;
            }
        }

        public void Update(Population population)
        {
            base.Update();
            for (int i = 0; i < population.Data!.Length; i++)
            {
                if (!population.Data[i].IsDead && IsColliding(population.Data[i]))
                {
                    population.Data[i].IsDead = true;
                    population.Alives--;
                }
            }
        }

        public bool IsColliding(Sonic sonic)
        {
            return Obstacles.Exists(obstacle => obstacle.IsColliding(sonic));
        }

        protected override void SetNextSpawnInterval()
        {
            _nextSpawnInterval = _random.Next(-31, 31);
        }

        protected override void CheckTimer()
        {
            if (_nextSpawnInterval > 111 - _gameSpeed)
            {
                if (_countObstacles < 3 && _random.NextDouble() < 0.3)
                {
                    _gameObjects.Add(ObstacleFactory.CreateObstacle(_gameSpeed, ObstacleType.Bat));
                }
                else
                {
                    _gameObjects.Add(ObstacleFactory.CreateObstacle(_gameSpeed));
                }
                _countObstacles++;
                SetNextSpawnInterval();
            }

            _nextSpawnInterval++;
        }

        public void Reset()
        {
            _gameObjects.Clear();
            _nextSpawnInterval = 50;
            _countObstacles = 0;
        }
    }
}