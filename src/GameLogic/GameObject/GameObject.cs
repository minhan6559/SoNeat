using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    // Base game object class
    public abstract class GameObject
    {
        public float X { get; set; } // X position
        public float Y { get; set; } // Y position
        public float GameSpeed { get; set; } // Game speed
        public float Speed { get; set; } // Object speed
        protected MySprite Sprite { get; set; } // Object sprite
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

        // Move object
        public virtual void Move()
        {
            X -= Speed;
        }

        // Check if object is off screen
        public virtual bool IsOffScreen()
        {
            return X + CurrentBitmap.Width < 0;
        }
    }
}