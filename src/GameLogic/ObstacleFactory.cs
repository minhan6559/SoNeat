using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public enum ObstacleType
    {
        Crab,
        Bat,
        Spike
    }

    public class ObstacleFactory
    {
        private static readonly Random _random = new Random();
        public static Obstacle CreateObstacle(float gameSpeed, ObstacleType? type = null)
        {
            if (type == null)
            {
                type = (ObstacleType)_random.Next(0, Enum.GetValues(typeof(ObstacleType)).Length);
            }

            string folderPath = Utility.NormalizePath("assets/images/" + type.ToString());

            return type switch
            {
                ObstacleType.Crab => new Crab(SplashKit.ScreenWidth(), 560, gameSpeed, gameSpeed, folderPath),
                ObstacleType.Bat => CreateBatObstacle(gameSpeed, folderPath),
                ObstacleType.Spike => new Spike(SplashKit.ScreenWidth(), 583, gameSpeed, gameSpeed, folderPath),
                _ => throw new ArgumentException("Invalid obstacle type")
            };
        }

        // Create Bat obstacle randomly
        public static Obstacle CreateBatObstacle(float gameSpeed, string folderPath)
        {
            float randomY;

            if (_random.NextDouble() < 0.5)
            {
                randomY = 465;
            }
            else
            {
                randomY = 348;
            }

            float randomSpeed = (float)(_random.NextDouble() * (0.25 * gameSpeed) + gameSpeed);

            return new Bat(SplashKit.ScreenWidth(), randomY, randomSpeed, gameSpeed, folderPath);
        }
    }
}