using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class Bat : Obstacle
    {
        public const string DEFAULT_FOLDER_PATH = "assets/images/Bat";
        private float _initialY;
        private float _maxSpriteHeight;
        public Bat(float x, float y, float speed, float gameSpeed, string folderPath = DEFAULT_FOLDER_PATH)
                    : base(x, y, speed, gameSpeed, folderPath)
        {
            _initialY = y;

            _maxSpriteHeight = Sprite.Animations["Fly"].Max(bitmap => bitmap.Height);
        }

        public override void Update()
        {
            base.Update();

            Y = _initialY + (_maxSpriteHeight - CurrentBitmap.Height);
        }

        public override void UpdateGameSpeed(float gameSpeed)
        {
            base.UpdateGameSpeed(gameSpeed);

            Speed = gameSpeed;
        }
    }
}