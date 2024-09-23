using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class Agent
    {
        private Genome _genome;
        private double _fitness;


        public Agent(Genome genome)
        {
            _genome = genome;
            _fitness = 0.0;
        }

        public Agent(int inputSize, int outputSize)
        {
            _genome = new Genome(inputSize, outputSize, false);
            _fitness = 0.0;
        }

        public Genome Genome
        {
            get => _genome;
            set => _genome = value;
        }

        public double Fitness
        {
            get => _fitness;
            set => _fitness = value;
        }

        public double[]? FeedForward(params double[] inputs)
        {
            return _genome.FeedForward(inputs);
        }

        public Agent Crossover(Agent other)
        {
            Agent child = new Agent(_genome.CrossOver(other._genome));
            child.Genome.CreateNetwork();
            return child;
        }

        public void Mutate(List<ConnectionHistory> innovationHistory)
        {
            _genome.Mutate(innovationHistory);
            _genome.CreateNetwork();
        }

        public Agent Clone()
        {
            Agent child = new Agent(_genome.Clone());
            child.Genome.CreateNetwork();
            return child;
        }
    }
}