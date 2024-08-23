using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.GameLogic;

namespace SoNeat.src.Screen
{
    public class TestScreenState : IScreenState
    {
        private MySprite _sonic;
        public void EnterState()
        {
            _sonic = new MySprite("assets\\images\\Sonic", 52, 509);
        }

        public void Update()
        {
            // Handle game logic, player input, and updates
            _sonic.Update();
        }

        public void Draw()
        {
            // Draw the game elements to the screen
            _sonic.Draw();
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Test Screen State");
        }
    }
}