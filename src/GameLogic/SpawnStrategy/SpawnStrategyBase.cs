using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Updated spawn strategies to use the generic factory
    public abstract class SpawnStrategyBase : ISpawnStrategy
    {
        private IGameObjectFactory _factory;
        protected IGameObjectFactory Factory => _factory;

        protected SpawnStrategyBase(IGameObjectFactory factory)
        {
            _factory = factory;
        }

        public abstract GameObject CreateGameObject(float gameSpeed, float screenWidth);
        public abstract float CalculateNextSpawnInterval(Random random);
    }
}