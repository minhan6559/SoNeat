using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class ConnectionGene : Gene
    {
        private NodeGene _fromNode, _toNode;
        private double _weight;
        private bool _enabled;

        private int _replaceIndex;

        public ConnectionGene(NodeGene fromNode, NodeGene toNode)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = 0.0f;
            _enabled = true;
            _replaceIndex = -1;
        }

        public ConnectionGene(NodeGene fromNode, NodeGene toNode, double weight)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = true;
            _replaceIndex = -1;
        }

        public ConnectionGene(NodeGene fromNode, NodeGene toNode, double weight, bool enabled)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = enabled;
            _replaceIndex = -1;
        }
        public ConnectionGene(NodeGene fromNode, NodeGene toNode, double weight, bool enabled, int innovationNum) : base(innovationNum)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = enabled;
            _replaceIndex = -1;
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

        public int ReplaceIndex
        {
            get => _replaceIndex;
            set => _replaceIndex = value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not ConnectionGene)
                return false;

            ConnectionGene connection = (ConnectionGene)obj;
            return _fromNode.Equals(connection.FromNode) && _toNode.Equals(connection.ToNode);
        }

        public override int GetHashCode()
        {
            return _fromNode.GetHashCode() * Neat.MAX_NODES + _toNode.GetHashCode();
        }

        public override Gene Clone()
        {
            return new ConnectionGene(_fromNode, _toNode, _weight, _enabled, InnovationNum);
        }
    }
}