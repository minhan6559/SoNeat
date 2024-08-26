using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.Screen
{
    // Singleton
    public class ScreenManager
    {
        private static ScreenManager? _instance;
        private static readonly object _lock = new object();
        private IScreenState? _currentState;

        // Private constructor to prevent instantiation
        private ScreenManager() { }

        // Public static method to get the instance of ScreenManager
        public static ScreenManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new ScreenManager();
                    }
                }
                return _instance;
            }
        }

        public void SetState(IScreenState state)
        {
            _currentState?.ExitState();
            _currentState = state;
            _currentState.EnterState();
        }

        public void Update()
        {
            _currentState?.Update();
        }

        public void Draw()
        {
            _currentState?.Draw();
        }
    }

}