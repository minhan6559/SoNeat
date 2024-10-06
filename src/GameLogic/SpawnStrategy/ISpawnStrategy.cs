using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Strategy Pattern for spawn behavior
    public interface ISpawnStrategy
    {
        GameObject CreateGameObject(float gameSpeed, float screenWidth);
        float CalculateNextSpawnInterval(Random random);
    }
}