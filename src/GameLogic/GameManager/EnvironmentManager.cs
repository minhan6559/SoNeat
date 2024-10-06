using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class EnvironmentManager : GameManagerBase
    {
        public EnvironmentManager(float gameSpeed)
            : base(gameSpeed, new CloudSpawnStrategy())
        {
            nextSpawnInterval = 150;
            GameObjectFactory factory = new GameObjectFactory();
            gameObjects.Add(factory.CreateGameObject(GameObjectType.Ground, gameSpeed, 0, 634));
        }

        protected override bool ShouldSpawn()
        {
            return nextSpawnInterval >= 180;
        }
    }
}