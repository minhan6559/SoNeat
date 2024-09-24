using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class Node : IComparable<Node>
    {
        private double _x, _output;
        private List<Connection> _connections;

        public Node(double x)
        {
            _x = x;
            _output = 0.0f;
            _connections = new List<Connection>();
        }

        public double X
        {
            get => _x;
            set => _x = value;
        }

        public double Output
        {
            get => _output;
            set => _output = value;
        }

        public List<Connection> Connections
        {
            get => _connections;
            set => _connections = value;
        }

        public void FeedForward()
        {
            double s = 0.0f;
            foreach (Connection conn in _connections)
            {
                if (conn.Enabled)
                    s += conn.FromNode.Output * conn.Weight;
            }
            _output = Sigmoid(s);
        }

        public double Sigmoid(double input)
        {
            return 1.0f / (1.0f + Math.Exp(-input));
        }

        public int CompareTo(Node? other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }
            return _x.CompareTo(other._x);
        }
    }
}