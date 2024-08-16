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
        private RandomHashSet<NodeGene> _nodesHashSet;

        private int _inputSize, _outputSize, _clients;

        public Neat(int inputSize, int outputSize, int clients)
        {
            _connectionsMap = new Dictionary<ConnectionGene, ConnectionGene>();
            _nodesHashSet = new RandomHashSet<NodeGene>();
            Reset(inputSize, outputSize, clients);
        }

        public void Reset(int inputSize, int outputSize, int clients)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;
            _clients = clients;

            _connectionsMap.Clear();
            _nodesHashSet.Clear();

            for (int i = 0; i < _inputSize; i++)
            {
                NodeGene node = CreateNode();
                node.X = 0.1;
                node.Y = (i + 1) / (double)(_inputSize + 1);
            }

            for (int i = 0; i < _outputSize; i++)
            {
                NodeGene node = CreateNode();
                node.X = 0.9;
                node.Y = (i + 1) / (double)(_outputSize + 1);
            }
        }
        public Genome.Genome CreateEmptyGenome()
        {
            Genome.Genome genome = new(this);
            for (int i = 0; i < _inputSize + _outputSize; i++)
                genome.Nodes.Add(GetNode(i + 1));

            return genome;
        }

        public NodeGene CreateNode()
        {
            if (_nodesHashSet.Count >= MAX_NODES)
                return null!;

            NodeGene node = new NodeGene(_nodesHashSet.Count + 1);
            _nodesHashSet.Add(node);

            return node;
        }
        public NodeGene GetNode(int index)
        {
            if (index > 0 || index <= _nodesHashSet.Count)
                return _nodesHashSet.GetAt(index - 1);

            return CreateNode();
        }

        public ConnectionGene GetConnection(NodeGene fromNode, NodeGene toNode)
        {
            ConnectionGene connection = new(fromNode, toNode);
            if (_connectionsMap.ContainsKey(connection))
                connection.InnovationNum = _connectionsMap[connection].InnovationNum;
            else
            {
                _connectionsMap.Add(connection, connection);
                connection.InnovationNum = _connectionsMap.Count + 1;
            }

            return connection;
        }
        public ConnectionGene GetConnection(ConnectionGene gene)
        {
            ConnectionGene connection = new(gene.FromNode, gene.ToNode);
            connection.Weight = gene.Weight;
            connection.Enabled = gene.Enabled;
            return connection;
        }
    }
}