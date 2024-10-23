using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Obstacle spawn strategy class for spawning obstacle objects
    public class ObstacleSpawnStrategy : SpawnStrategyBase
    {
        private readonly Random _random = new Random();

        public ObstacleSpawnStrategy(IGameObjectFactory factory) : base(factory)
        {
        }

        // Create game object
        public override GameObject CreateGameObject(float gameSpeed, float screenWidth)
        {
            GameObjectType type = GetRandomObstacleType();
            return Factory.CreateGameObject(
                type,
                gameSpeed,
                screenWidth + 100,
                0); // Y position will be adjusted in factory based on type
        }

        public override float CalculateNextSpawnInterval(Random random)
        {
            return random.Next(-31, 31);
        }

        private GameObjectType GetRandomObstacleType()
        {
            var obstacles = new[]
            {
                GameObjectType.Crab,
                GameObjectType.Spike,
                GameObjectType.Hog,
                GameObjectType.Bat
            };
            return obstacles[_random.Next(obstacles.Length)];
        }
    }

}