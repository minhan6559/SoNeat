using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public abstract class Obstacle : GameObject
    {
        public bool HasPassedPlayer { get; set; }
        public Obstacle(float x, float y, float speed, float gameSpeed, string folderPath)
                    : base(x, y, speed, gameSpeed, folderPath)
        {
            HasPassedPlayer = false;
        }

        public virtual void CheckPassedPlayer(Sonic sonic)
        {
            if (X + CurrentBitmap.Width < sonic.X)
            {
                HasPassedPlayer = true;
            }
        }
    }
}