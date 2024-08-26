using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class Crab : Obstacle
    {
        public Crab(float x, float y, float gameSpeed, string folderPath = "assets\\images\\Crab")
                    : base(x, y, gameSpeed, folderPath)
        {

        }

        public override void Move()
        {
            X -= GameSpeed;
        }
    }
}