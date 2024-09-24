using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class Connection
    {
        private Node _fromNode, _toNode;
        private double _weight;
        private bool _enabled;

        public Connection(Node fromNode, Node toNode)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = 0.0f;
            _enabled = true;
        }

        public Connection(Node fromNode, Node toNode, double weight)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = true;
        }

        public Connection(Node fromNode, Node toNode, double weight, bool enabled)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = enabled;
        }

        public Node FromNode
        {
            get => _fromNode;
            set => _fromNode = value;
        }

        public Node ToNode
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
    }
}