using System;
using SplashKitSDK;
using SoNeat.src.NEAT.DataStructures;
using SoNeat.src.NEAT.Gene;
using SoNeat.src.NEAT.NeuralEvolution;

using SoNeat.src.Screen;
using SoNeat.src.GameLogic;

namespace SoNeat
{
    public class Program
    {
        private static readonly Color BACKGROUND_COLOR = Color.RGBColor(132, 204, 234);
        public static void Main()
        {
            Window window = new Window("SoNeat", 1250, 720);
            ScreenManager.Instance.SetState(new GameScreenState());

            while (!window.CloseRequested)
            {
                SplashKit.ProcessEvents();
                ScreenManager.Instance.Update();

                window.Clear(BACKGROUND_COLOR);

                ScreenManager.Instance.Draw();
                SplashKit.RefreshScreen(60);
            }
        }
    }
}
