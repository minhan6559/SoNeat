using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NEATRex.src.NEAT.Gene;
using NEATRex.src.NEAT.Calculation;
using System.Runtime.InteropServices;

namespace NEATRex.src.NEAT.NeuralEvolution
{
    public class Agent : IComparable<Agent>
    {
        private Genome _genome;
        private Species? _species;
        private double _fitness;

        private Calculator? _calculator;

        public Agent(Genome genome)
        {
            _genome = genome;
            _species = null;
            _fitness = 0.0;
            _calculator = null;
        }

        public Genome Genome
        {
            get => _genome;
            set => _genome = value;
        }

        public Species? Species
        {
            get => _species;
            set => _species = value;
        }

        public double Fitness
        {
            get => _fitness;
            set => _fitness = value;
        }

        public void CreateCalculator()
        {
            _calculator = new Calculator(_genome);
        }

        public double[]? FeedForward(params double[] inputs)
        {
            if (_calculator != null)
                return _calculator.FeedForward(inputs);

            CreateCalculator();
            return null;
        }

        public double DistanceTo(Agent other)
        {
            return _genome.DistanceTo(other.Genome);
        }

        public void Mutate()
        {
            _genome.Mutate();
        }

        public int CompareTo(Agent? other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return _fitness.CompareTo(other._fitness);
        }
    }
}