using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public abstract class GameObject
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float GameSpeed { get; set; }
        public float Speed { get; set; }
        protected MySprite Sprite { get; set; }
        public Bitmap CurrentBitmap => Sprite.CurrentBitmap;

        public GameObject(float x, float y, float speed, float gameSpeed, string folderPath)
        {
            X = x;
            Y = y;
            Speed = speed;
            GameSpeed = gameSpeed;
            Sprite = new MySprite(folderPath);
        }

        public virtual void Update()
        {
            Move();
            Sprite.Update();
        }

        public virtual void UpdateGameSpeed(float gameSpeed)
        {
            GameSpeed = gameSpeed;
        }

        public virtual void Draw()
        {
            Sprite.Draw(X, Y);
        }

        public virtual void DrawOutline()
        {
            SplashKit.DrawRectangle(Color.Red, X, Y, CurrentBitmap.Width, CurrentBitmap.Height);
        }

        public virtual void PlayAnimation(string animation)
        {
            Sprite.Play(animation);
        }

        public virtual void Move()
        {
            X -= Speed;
        }

        public virtual bool IsOffScreen()
        {
            return X + CurrentBitmap.Width < 0;
        }
    }
}