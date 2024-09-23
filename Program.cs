using System;
using SplashKitSDK;
using SoNeat.src.NEAT.DataStructures;
using SoNeat.src.NEAT.Gene;
using SoNeat.src.NEAT.NeuralEvolution;

using SoNeat.src.Screen;
using SoNeat.src.Utils;

namespace SoNeat
{
    public class Program
    {
        private static readonly Color BACKGROUND_COLOR = Color.RGBColor(132, 204, 234);
        public static void Main()
        {
            Window window = new Window("SoNeat", 1250, 720);
            ScreenManager.Instance.SetState(new TestScreenState());
            SplashKit.LoadFont("MainFont", Utility.NormalizePath("assets/fonts/PressStart2P.ttf"));

            while (true)
            {
                SplashKit.ProcessEvents();
                ScreenManager.Instance.Update();

                if (SplashKit.WindowCloseRequested(window))
                {
                    break;
                }

                window.Clear(BACKGROUND_COLOR);

                ScreenManager.Instance.Draw();
                SplashKit.RefreshScreen(60);
            }

            Console.WriteLine("Goodbye!");
        }
    }
}
