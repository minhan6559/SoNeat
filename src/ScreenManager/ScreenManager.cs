using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NEATRex.src.ScreenManager
{
    // Singleton
    public class ScreenManager
    {
        private static ScreenManager _instance;
        private List<Screen> _screens;
        private Screen _currentScreen;

        private ScreenManager()
        {
            _screens = new List<Screen>();
        }

        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ScreenManager();
                }
                return _instance;
            }
        }

        public void AddScreen(Screen screen)
        {
            _screens.Add(screen);
        }

        public void RemoveScreen(Screen screen)
        {
            _screens.Remove(screen);
        }

        public void SetScreen(string name)
        {
            foreach (Screen screen in _screens)
            {
                if (screen.Name == name)
                {
                    _currentScreen = screen;
                    break;
                }
            }
        }

        public void Update()
        {
            _currentScreen.Update();
        }

        public void Draw()
        {
            _currentScreen.Draw();
        }
    }
}