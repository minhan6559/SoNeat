using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public class EnvironmentFactory : IGameObjectFactory
    {
        private readonly Random _random = new Random();

        public GameObject CreateGameObject(GameObjectType type, float gameSpeed, float xPosition, float yPosition)
        {
            return type switch
            {
                // Environment objects
                GameObjectType.Ground => new Ground(xPosition, yPosition, gameSpeed),
                GameObjectType.Cloud => CreateCloud(xPosition, yPosition),

                _ => throw new ArgumentException($"Unknown game object type: {type}")
            };
        }

        private GameObject CreateCloud(float xPosition, float yPosition)
        {
            float randomSpeed = (float)(_random.NextDouble() * 1.0f + 3.0f);
            return new Cloud(xPosition, yPosition, randomSpeed);
        }
    }
}