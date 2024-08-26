using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class Ground : GameObject
    {
        public Ground(float x, float y, float gameSpeed, string folderPath = "assets\\images\\Ground")
                    : base(x, y, gameSpeed, folderPath)
        {

        }

        public override void Update()
        {
            Move();
        }

        public override void Move()
        {
            X -= GameSpeed;
            if (X + CurrentBitmap.Width < 0)
            {
                X = 0;
            }
        }

        public override void Draw()
        {
            Sprite.Draw(X, Y);
            Sprite.Draw(X + CurrentBitmap.Width, Y);
        }
    }
}