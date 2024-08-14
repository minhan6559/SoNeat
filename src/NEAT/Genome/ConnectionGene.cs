using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NEATRex.src.NEAT.NeuralEvolution;

namespace NEATRex.src.NEAT.Genome
{
    public class ConnectionGene : Gene
    {
        private NodeGene _fromNode, _toNode;
        private double _weight;
        private bool _enabled;

        public ConnectionGene(NodeGene fromNode, NodeGene toNode)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = 0.0f;
            _enabled = true;
        }

        public NodeGene FromNode
        {
            get => _fromNode;
            set => _fromNode = value;
        }

        public NodeGene ToNode
        {
            get => _toNode;
            set => _toNode = value;
        }

        public double Weight
        {
            get => _weight;
            set => _weight = value;
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public override int HashCode => _fromNode.HashCode * Neat.MAX_NODES + _toNode.HashCode;

        public override bool IsEqual(object obj)
        {
            if (obj is not ConnectionGene)
                return false;

            ConnectionGene connection = (ConnectionGene)obj;
            return _fromNode.IsEqual(connection.FromNode) && _toNode.IsEqual(connection.ToNode);
        }
    }
}