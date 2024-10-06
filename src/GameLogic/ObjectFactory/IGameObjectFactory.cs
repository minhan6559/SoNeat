using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Enum for all possible game object types
    public enum GameObjectType
    {
        // Environment objects
        Ground,
        Cloud,

        // Obstacles
        Crab,
        Spike,
        Hog,
        Bat
    }

    // Generic Factory interface for all game objects
    public interface IGameObjectFactory
    {
        GameObject CreateGameObject(GameObjectType type, float gameSpeed, float xPosition, float yPosition);
    }
}