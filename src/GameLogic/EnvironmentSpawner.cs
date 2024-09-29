using System;
using System.Collections.Generic;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public class EnvironmentSpawner : RandomSpawnerBase
    {
        public EnvironmentSpawner(float gameSpeed) : base(gameSpeed)
        {
            _nextSpawnInterval = 150;
            _gameObjects.Add(new Ground(0, 634, gameSpeed));
        }

        private void AddCloud()
        {
            float randomY = _random.Next(55, 260);
            float randomSpeed = (float)(_random.NextDouble() * 1.0f + 3.0f);

            _gameObjects.Add(new Cloud(SplashKit.ScreenWidth(), randomY, randomSpeed));
        }

        protected override void SetNextSpawnInterval()
        {
            _nextSpawnInterval = _random.Next(-50, 50);
        }

        protected override void CheckTimer()
        {
            if (_nextSpawnInterval >= 180)
            {
                AddCloud();
                SetNextSpawnInterval();
            }
            _nextSpawnInterval++;
        }
    }
}