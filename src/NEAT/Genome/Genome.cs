using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using NEATRex.src.NEAT.DataStructures;
using NEATRex.src.NEAT.NeuralEvolution;

namespace NEATRex.src.NEAT.Genome
{
    public class Genome
    {
        private RandomHashSet<ConnectionGene> _connections;
        private RandomHashSet<NodeGene> _nodes;

        private Neat _neat;

        public Genome(Neat neat)
        {
            _connections = new RandomHashSet<ConnectionGene>();
            _nodes = new RandomHashSet<NodeGene>();
            _neat = neat;
        }

        public RandomHashSet<ConnectionGene> Connections => _connections;
        public RandomHashSet<NodeGene> Nodes => _nodes;
        public Neat Neat => _neat;

        public double DistanceTo(Genome other)
        {
            return 0;
        }

        public Genome Crossover(Genome other)
        {
            return null;
        }

        public void Mutate()
        {
        }
    }
}