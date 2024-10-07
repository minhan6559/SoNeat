using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public abstract class Obstacle : GameObject
    {
        public Obstacle(float x, float y, float speed, float gameSpeed, string folderPath)
                    : base(x, y, speed, gameSpeed, folderPath)
        {
        }

        public virtual bool HasPassedPlayer(Sonic sonic)
        {
            return X + CurrentBitmap.Width < sonic.X;
        }

        public virtual bool IsColliding(GameObject other)
        {
            return CurrentBitmap.BitmapCollision(X, Y, other.CurrentBitmap, other.X, other.Y);
        }
    }
}