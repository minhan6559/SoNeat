using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SplashKitSDK;

namespace SoNeat.src.GameLogic
{
    public class MySprite
    {
        private Dictionary<string, List<Bitmap>> _animations;
        private Dictionary<string, int> _animation_lengths;
        private string _current_animation;
        private int _current_animation_frame;
        private int _animation_timer;
        public float X { get; set; }
        public float Y { get; set; }

        public MySprite(string folderPath, float x, float y)
        {
            _animations = new Dictionary<string, List<Bitmap>>();
            _animation_lengths = new Dictionary<string, int>();
            _current_animation = "";
            _animation_timer = 0;
            _current_animation_frame = 0;

            X = x;
            Y = y;

            LoadSettings(folderPath);
        }

        public float Width => _animations.Values.First().First().Width;
        public float Height => _animations.Values.First().First().Height;

        private void LoadSettings(string folderPath)
        {
            string[] directories = Directory.GetDirectories(folderPath);
            foreach (string directory in directories)
            {
                string folderName = Path.GetFileName(directory);

                List<Bitmap> bitmaps = new List<Bitmap>();
                string[] files = Directory.GetFiles(directory);
                Array.Sort(files);

                foreach (string file in files)
                {
                    // If it is a png file, load it
                    if (Path.GetExtension(file) == ".png")
                    {
                        string BitmapName = Path.GetFileNameWithoutExtension(file);
                        if (SplashKit.HasBitmap(BitmapName))
                        {
                            bitmaps.Add(SplashKit.BitmapNamed(BitmapName));
                        }
                        else
                        {
                            bitmaps.Add(SplashKit.LoadBitmap(BitmapName, file));
                        }
                    }

                    if (Path.GetExtension(file) == ".txt")
                    {
                        string[] lines = File.ReadAllLines(file);

                        // The first line is the animation length
                        _animation_lengths.Add(folderName, int.Parse(lines[0]));
                    }
                }

                _animations.Add(folderName, bitmaps);
            }

            _current_animation = _animations.Keys.First();
        }

        public void Update()
        {
            _animation_timer++;
            if (_animation_timer >= _animation_lengths[_current_animation])
            {
                _animation_timer = 0;
                _current_animation_frame = (_current_animation_frame + 1) % _animations[_current_animation].Count;
            }
        }

        public void Draw()
        {
            _animations[_current_animation][_current_animation_frame].Draw(X, Y);
        }
    }
}