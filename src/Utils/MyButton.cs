using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;

namespace SoNeat.src.Utils
{
    // Class for creating buttons
    public class MyButton
    {
        private Bitmap _buttonBitmap; // Bitmap of the button
        public double X { get; set; }
        public double Y { get; set; }

        public MyButton(string bitmapPath, double x, double y)
        {
            bitmapPath = Utility.NormalizePath(bitmapPath);
            string BitmapName = Path.GetFileNameWithoutExtension(bitmapPath);
            if (SplashKit.HasBitmap(BitmapName))
            {
                _buttonBitmap = SplashKit.BitmapNamed(BitmapName);
            }
            else
            {
                _buttonBitmap = SplashKit.LoadBitmap(BitmapName, bitmapPath);
            }
            X = x;
            Y = y;
        }

        // Draw the button
        public void Draw()
        {
            SplashKit.DrawBitmap(_buttonBitmap, X, Y);
        }

        // Check if the button is clicked
        public bool IsClicked()
        {
            if (SplashKit.MouseClicked(MouseButton.LeftButton))
            {
                Point2D pt = SplashKit.MousePosition();
                return (pt.X >= X) && (pt.X <= (X + _buttonBitmap.Width))
                    && (pt.Y >= Y) && (pt.Y <= (Y + _buttonBitmap.Height));
            }
            return false;
        }

        // Check if the button is hovered
        public bool IsHovered()
        {
            Point2D pt = SplashKit.MousePosition();
            return (pt.X >= X) && (pt.X <= (X + _buttonBitmap.Width))
                && (pt.Y >= Y) && (pt.Y <= (Y + _buttonBitmap.Height));
        }
    }
}