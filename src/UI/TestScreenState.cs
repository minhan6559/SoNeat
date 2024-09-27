using System;
using System.Collections.Generic;
using System.Linq;

using SoNeat.src.GameLogic;
using SoNeat.src.NEAT;
using SplashKitSDK;
using SoNeat.src.Utils;

namespace SoNeat.src.UI
{
    public class TestScreenState : IScreenState
    {
        public void EnterState()
        {
            Neat neat = new Neat(10, 1, 1000);

            double[] input = new double[10];
            Random random = new Random();
            for (int i = 0; i < 10; i++) input[i] = random.NextDouble();

            for (int i = 0; i < 100; i++)
            {
                foreach (Agent c in neat.Agents)
                {
                    double score = c.FeedForward(input)![0];
                    c.Fitness = score;
                }
                neat.Evolve();
                neat.PrintSpecies();
            }
        }

        public void Update()
        {

        }

        public void Draw()
        {
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}