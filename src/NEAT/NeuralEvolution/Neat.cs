using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEATRex.src.NEAT.Genome;
using NEATRex.src.NEAT.DataStructures;

namespace NEATRex.src.NEAT.NeuralEvolution
{
    public class Neat
    {
        public readonly static int MAX_NODES = (int)Math.Pow(2, 20);

        private Dictionary<ConnectionGene, ConnectionGene> _connectionsMap;
        private RandomHashSet<ConnectionGene> _nodesHashSet;

        public Neat(int inputSize, int outputSize, int clients)
        {
            Reset(inputSize, outputSize, clients);
            _connectionsMap = new Dictionary<ConnectionGene, ConnectionGene>();
            _nodesHashSet = new RandomHashSet<ConnectionGene>();
        }

        public void Reset(int inputSize, int outputSize, int clients)
        {
            _connectionsMap.Clear();
            _nodesHashSet.Reset();
        }
    }
}