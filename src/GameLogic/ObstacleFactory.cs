using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public enum ObstacleType
    {
        Crab,
        Bird
    }

    public class ObstacleFactory
    {
        public static Obstacle CreateObstacle(ObstacleType type, float x, float y, float gameSpeed)
        {
            string spritesFolderPath = Path.Combine("assets", "images");
            string folderPath = Path.Combine(spritesFolderPath, type.ToString());

            return type switch
            {
                ObstacleType.Crab => new Crab(x, y, gameSpeed, folderPath),
                _ => throw new ArgumentException("Invalid obstacle type")
            };
        }
    }
}