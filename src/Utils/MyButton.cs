using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;

namespace SoNeat.src.Utils
{
    public class MyButton
    {
        public Bitmap ButtonBitmap { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public MyButton(string bitmapPath, double x, double y)
        {
            bitmapPath = Utility.NormalizePath(bitmapPath);
            string BitmapName = Path.GetFileNameWithoutExtension(bitmapPath);
            if (SplashKit.HasBitmap(BitmapName))
            {
                ButtonBitmap = SplashKit.BitmapNamed(BitmapName);
            }
            else
            {
                ButtonBitmap = SplashKit.LoadBitmap(BitmapName, bitmapPath);
            }
            X = x;
            Y = y;
        }

        public void Draw()
        {
            SplashKit.DrawBitmap(ButtonBitmap, X, Y);
        }

        public bool IsClicked()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D pt = SplashKit.MousePosition();
                return (pt.X >= X) && (pt.X <= (X + ButtonBitmap.Width))
                    && (pt.Y >= Y) && (pt.Y <= (Y + ButtonBitmap.Height));
            }
            return false;
        }

        public bool IsHovered()
        {
            Point2D pt = SplashKit.MousePosition();
            return (pt.X >= X) && (pt.X <= (X + ButtonBitmap.Width))
                && (pt.Y >= Y) && (pt.Y <= (Y + ButtonBitmap.Height));
        }
    }
}