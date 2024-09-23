using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

using SplashKitSDK;
using SoNeat.src.Utils;
using System.Text.RegularExpressions;

namespace SoNeat.src.GameLogic
{
    public enum ObstacleType
    {
        Crab,
        Bat,
        Spike,
        Hog
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
                ObstacleType.Bat => new Bat(SplashKit.ScreenWidth(), CreateRandomBatY(), CreateRandomBatSpeed(gameSpeed), gameSpeed, folderPath),
                ObstacleType.Spike => new Spike(SplashKit.ScreenWidth(), 583, gameSpeed, gameSpeed, folderPath),
                ObstacleType.Hog => new Hog(SplashKit.ScreenWidth(), 459, gameSpeed, gameSpeed, folderPath),
                _ => throw new ArgumentException("Invalid obstacle type")
            };
        }

        // Create Bat Speed randomly
        private static float CreateRandomBatSpeed(float gameSpeed)
        {
            return (float)(_random.NextDouble() * (0.25 * gameSpeed) + gameSpeed);
        }

        // Create Bat Y randomly
        private static float CreateRandomBatY()
        {
            if (_random.NextDouble() < 0.65)
            {
                return 348;
            }
            else
            {
                return 465;
            }
        }
    }
}