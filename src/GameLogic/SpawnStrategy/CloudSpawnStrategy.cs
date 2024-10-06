using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class CloudSpawnStrategy : SpawnStrategyBase
    {
        public CloudSpawnStrategy() : base()
        {
        }

        public override GameObject CreateGameObject(float gameSpeed, float screenWidth)
        {
            float randomY = new Random().Next(55, 260);
            return factory.CreateGameObject(
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