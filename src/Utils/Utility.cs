using SplashKitSDK;

namespace SoNeat.src.Utils
{
    public class Utility
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return "";

            // Determine the correct separator for the current OS
            char correctSeparator = Path.DirectorySeparatorChar;
            char incorrectSeparator = (correctSeparator == '/') ? '\\' : '/';

            // Replace incorrect separators with the correct ones
            return path.Replace(incorrectSeparator, correctSeparator);
        }

        public static double Sigmoid(double x)
        {
            return 1.0f / (1.0f + Math.Exp(-4.9 * x));
        }

        public static double RandomGaussian()
        {
            Random _random = new Random();
            double u1 = 1.0 - _random.NextDouble(); // uniform(0,1) random number
            double u2 = 1.0 - _random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);  // standard normal distributed value
            return randStdNormal;
        }

        public static double Normalize(double value, double min, double max, double newMin, double newMax)
        {
            if (value < min)
                return newMin;
            if (value > max)
                return newMax;
            return newMin + (value - min) * (newMax - newMin) / (max - min);
        }

        public static void FadeToNewMusic(string newMusic, int fadeInMs, float newVolume)
        {
            Music gameMusic = SplashKit.MusicNamed(newMusic);
            SplashKit.FadeMusicIn(gameMusic, -1, fadeInMs);
            SplashKit.SetMusicVolume(newVolume);
        }
    }
}