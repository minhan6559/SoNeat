using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.UI
{
    // Singleton
    public class ScreenManager
    {
        private static ScreenManager? _instance;
        private static readonly object _lock = new object();
        private IScreenState? _currentState;

        // Global FrameRate property
        private static int _frameRate = 60; // Default value
        public static int FrameRate
        {
            get { return _frameRate; }
            set { _frameRate = value; }
        }

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
            if (_currentState != null)
                _currentState.ExitState();
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