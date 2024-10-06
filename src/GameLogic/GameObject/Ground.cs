using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    public class Ground : GameObject
    {
        public const string DEFAULT_FOLDER_PATH = "assets/images/Ground";
        private float _groundX1, _groundX2;

        public Ground(float x, float y, float gameSpeed, string folderPath = DEFAULT_FOLDER_PATH)
                    : base(x, y, gameSpeed, gameSpeed, folderPath)
        {
            _groundX1 = x;
            _groundX2 = x + CurrentBitmap.Width;
        }

        public override void Update()
        {
            Move();
        }

        public override void Move()
        {
            _groundX1 -= GameSpeed;
            _groundX2 -= GameSpeed;

            if (_groundX1 + CurrentBitmap.Width <= 0)
            {
                _groundX1 = _groundX2 + CurrentBitmap.Width;
            }

            if (_groundX2 + CurrentBitmap.Width <= 0)
            {
                _groundX2 = _groundX1 + CurrentBitmap.Width;
            }
        }

        public override void Draw()
        {
            Sprite.Draw(_groundX1, Y);
            Sprite.Draw(_groundX2, Y);
        }

        public override bool IsOffScreen()
        {
            return false;
        }
    }
}