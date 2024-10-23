using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Cloud spawn strategy class for spawning cloud objects
    public class CloudSpawnStrategy : SpawnStrategyBase
    {
        public CloudSpawnStrategy(IGameObjectFactory factory) : base(factory)
        {
        }

        // Create game object
        public override GameObject CreateGameObject(float gameSpeed, float screenWidth)
        {
            float randomY = new Random().Next(55, 260);
            return Factory.CreateGameObject(
                GameObjectType.Cloud,
                gameSpeed,
                screenWidth,
                randomY
            );
        }

        public override float CalculateNextSpawnInterval(Random random)
        {
            return random.Next(-50, 50);
        }
    }

}