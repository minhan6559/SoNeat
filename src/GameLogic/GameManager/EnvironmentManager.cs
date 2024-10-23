using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Environment manager class for managing environment objects
    public class EnvironmentManager : ObjectManagerBase
    {
        public EnvironmentManager(float gameSpeed)
            : base(gameSpeed, new CloudSpawnStrategy(new EnvironmentFactory()))
        {
            NextSpawnInterval = 150;
            EnvironmentFactory factory = new EnvironmentFactory();

            // Add ground object
            GameObjects.Add(factory.CreateGameObject(GameObjectType.Ground, gameSpeed, 0, 634));
        }

        protected override bool ShouldSpawn()
        {
            return NextSpawnInterval >= 180;
        }
    }
}