using System;
using System.Collections.Generic;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public class EnvironmentManager
    {
        private Ground? _ground;
        private List<Cloud> _clouds;
        private readonly SplashKitSDK.Timer _cloudTimer;
        private double _nextCloudInterval;
        private static readonly Random _random = new Random();

        public EnvironmentManager(float gameSpeed)
        {
            _ground = new Ground(0, 634, gameSpeed);
            _clouds = new List<Cloud>();
            _cloudTimer = new SplashKitSDK.Timer("Cloud Timer");
            _cloudTimer.Start();
            _nextCloudInterval = 0;
        }

        public void Update()
        {
            _ground!.Update();

            for (int i = 0; i < _clouds.Count; i++)
            {
                _clouds[i].Update();

                if (_clouds[i].IsOffScreen())
                {
                    _clouds.RemoveAt(i);
                    i--;
                }
            }

            // Check if it's time to create a new cloud
            if (_cloudTimer.Ticks > _nextCloudInterval)
            {
                AddCloud();
                _cloudTimer.Reset();
                SetNextCloudInterval();
            }
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            _ground!.UpdateGameSpeed(gameSpeed);

            foreach (Cloud cloud in _clouds)
            {
                cloud.UpdateGameSpeed(gameSpeed);
            }
        }

        private void AddCloud()
        {
            float randomY = _random.Next(55, 260);
            float randomSpeed = (float)(_random.NextDouble() * 1.0f + 3.0f);

            _clouds.Add(new Cloud(SplashKit.ScreenWidth(), randomY, randomSpeed));
        }

        private void SetNextCloudInterval()
        {
            _nextCloudInterval = _random.NextDouble() * 1000 + 2000;
        }

        public void Draw()
        {
            _ground!.Draw();
            foreach (Cloud cloud in _clouds)
            {
                cloud.Draw();
            }
        }
    }
}