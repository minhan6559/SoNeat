using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    // Concrete factory implementation
    public class GameObjectFactory : IGameObjectFactory
    {
        private readonly Random random = new Random();

        public GameObject CreateGameObject(GameObjectType type, float gameSpeed, float xPosition, float yPosition)
        {
            return type switch
            {
                // Environment objects
                GameObjectType.Ground => new Ground(xPosition, yPosition, gameSpeed),
                GameObjectType.Cloud => CreateCloud(xPosition, yPosition, gameSpeed),

                // Obstacles
                GameObjectType.Crab => CreateObstacle(type, xPosition, 560, gameSpeed),
                GameObjectType.Spike => CreateObstacle(type, xPosition, 583, gameSpeed),
                GameObjectType.Hog => CreateObstacle(type, xPosition, 459, gameSpeed),
                GameObjectType.Bat => CreateBat(xPosition, gameSpeed),

                _ => throw new ArgumentException($"Unknown game object type: {type}")
            };
        }

        private GameObject CreateCloud(float xPosition, float yPosition, float gameSpeed)
        {
            float randomSpeed = (float)(random.NextDouble() * 1.0f + 3.0f);
            return new Cloud(xPosition, yPosition, randomSpeed);
        }

        private GameObject CreateBat(float xPosition, float gameSpeed)
        {
            float y = random.NextDouble() < 0.5 ? 348 : 465;
            float speed = (float)(random.NextDouble() * (0.25 * gameSpeed) + gameSpeed);
            string folderPath = GetAssetPath(GameObjectType.Bat);
            return new Bat(xPosition, y, speed, gameSpeed, folderPath);
        }

        private GameObject CreateObstacle(GameObjectType type, float xPosition, float yPosition, float gameSpeed)
        {
            string folderPath = GetAssetPath(type);
            return type switch
            {
                GameObjectType.Crab => new Crab(xPosition, yPosition, gameSpeed, gameSpeed, folderPath),
                GameObjectType.Spike => new Spike(xPosition, yPosition, gameSpeed, gameSpeed, folderPath),
                GameObjectType.Hog => new Hog(xPosition, yPosition, gameSpeed, gameSpeed, folderPath),
                _ => throw new ArgumentException($"Invalid obstacle type: {type}")
            };
        }

        private string GetAssetPath(GameObjectType type) =>
            Utility.NormalizePath($"assets/images/{type}");
    }
}