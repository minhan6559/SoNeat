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
        public static void Main()
        {
            Window window = new Window("SoNeat", 1250, 720);
            ScreenManager.Instance.SetState(new TestScreenState());

            while (!window.CloseRequested)
            {
                SplashKit.ProcessEvents();
                ScreenManager.Instance.Update();

                window.Clear(Color.White);

                ScreenManager.Instance.Draw();
                SplashKit.RefreshScreen(60);
            }
        }
    }
}
