using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class Rock : Obstacle
    {
        public const string DEFAULT_FOLDER_PATH = "assets/images/Rock";
        public Rock(float x, float y, float speed, float gameSpeed, string folderPath = DEFAULT_FOLDER_PATH)
                    : base(x, y, speed, gameSpeed, folderPath)
        {

        }

        public override void Update()
        {
            Move();
        }

        public override void UpdateGameSpeed(float gameSpeed)
        {
            base.UpdateGameSpeed(gameSpeed);

            Speed = gameSpeed;
        }
    }
}