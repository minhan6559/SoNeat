using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public abstract class Obstacle : GameObject
    {
        public bool HasPassedPlayer { get; set; }
        public Obstacle(float x, float y, float gameSpeed, string folderPath)
                    : base(x, y, gameSpeed, folderPath)
        {
            HasPassedPlayer = false;
        }

        public virtual bool IsOffScreen()
        {
            return X + CurrentBitmap.Width < 0;
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