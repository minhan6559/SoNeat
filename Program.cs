using System;
// using SplashKitSDK;
using NEATRex.src.NEAT.DataStructures;
using NEATRex.src.NEAT.Gene;
using NEATRex.src.NEAT.NeuralEvolution;

namespace NEATRex
{
    public class Program
    {
        public static void Main()
        {
            Neat neat = new Neat(10, 1, 1000);

            double[] input = new double[10];
            Random random = new Random();
            for (int i = 0; i < 10; i++) input[i] = random.NextDouble();

            for (int i = 0; i < 100; i++)
            {
                foreach (Agent c in neat.Agents.Data)
                {
                    double[]? output = c.FeedForward(input);
                    if (output != null)
                    {
                        double score = output[0];
                        c.Fitness = score;
                    }
                }
                neat.Evolve();
                neat.PrintSpecies();
            }

            foreach (Agent c in neat.Agents.Data)
            {
                foreach (ConnectionGene g in c.Genome.Connections.Data)
                {
                    Console.Write(g.InnovationNum + " ");
                }
                Console.WriteLine();
            }
        }
    }
}
