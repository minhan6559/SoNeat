using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class ObstacleSpawnStrategy : SpawnStrategyBase
    {
        private readonly Random random = new Random();

        public ObstacleSpawnStrategy() : base()
        {
        }

        public override GameObject CreateGameObject(float gameSpeed, float screenWidth)
        {
            var type = GetRandomObstacleType();
            return factory.CreateGameObject(
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
            return obstacles[random.Next(obstacles.Length)];
        }
    }

}