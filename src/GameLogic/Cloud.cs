using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class Cloud : GameObject
    {
        public const string DEFAULT_FOLDER_PATH = "assets/images/Cloud";
        public Cloud(float x, float y, float speed, string folderPath = DEFAULT_FOLDER_PATH)
                    : base(x, y, speed, 0, folderPath)
        {
        }
    }
}