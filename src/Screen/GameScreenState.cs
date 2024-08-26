using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.GameLogic;

namespace SoNeat.src.Screen
{
    public class GameScreenState : IScreenState
    {
        public void EnterState()
        {

        }

        public void Update()
        {
            // Handle game logic, player input, and updates
        }

        public void Draw()
        {
            // Draw the game elements to the screen
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}