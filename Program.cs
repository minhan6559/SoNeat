using System;
using SplashKitSDK;
using SoNeat.src.NEAT;

using SoNeat.src.UI;
using SoNeat.src.UI.MainMenu;
using SoNeat.src.Utils;

namespace SoNeat
{
    public static class Program
    {
        private static readonly Color BACKGROUND_COLOR = Color.RGBColor(132, 204, 234);
        public static void Main()
        {
            Window window = new Window("SoNeat", 1250, 720);
            ScreenManager.Instance.SetState(new MainMenuState());
            SplashKit.LoadFont("MainFont", Utility.NormalizePath("assets/fonts/PressStart2P.ttf"));
            SplashKit.SetInterfaceFont("MainFont");
            SplashKit.SetInterfaceFontSize(12);

            while (true)
            {
                SplashKit.ProcessEvents();
                ScreenManager.Instance.Update();

                if (SplashKit.WindowCloseRequested(window))
                    break;

                window.Clear(BACKGROUND_COLOR);

                ScreenManager.Instance.Draw();
                SplashKit.RefreshScreen((uint)ScreenManager.FrameRate);
            }

            Console.WriteLine("Goodbye!");
        }
    }
}
