using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class Node
    {
        private int _innovationNum;
        private int _layer;
        private double _inputVal, _outputVal;
        public double X { get; set; } = 0;
        public double Y { get; set; } = 0;
        private List<Connection> _connections;

        public Node(int innovationNum)
        {
            _innovationNum = innovationNum;
            _layer = 0;
            _inputVal = 0;
            _outputVal = 0;
            _connections = new List<Connection>();
        }

        public int InnovationNum
        {
            get => _innovationNum;
            set => _innovationNum = value;
        }

        public int Layer
        {
            get => _layer;
            set => _layer = value;
        }

        public double InputVal
        {
            get => _inputVal;
            set => _inputVal = value;
        }

        public double OutputVal
        {
            get => _outputVal;
            set => _outputVal = value;
        }

        public List<Connection> Connections
        {
            get => _connections;
            set => _connections = value;
        }

        public double Sigmoid(double x)
        {
            return 1.0f / (1.0f + Math.Exp(-4.9 * x));
        }

        public void FeedForward()
        {
            if (_layer != 0)
            {
                _outputVal = Sigmoid(_inputVal);
            }

            for (int i = 0; i < _connections.Count; i++)
            {
                if (_connections[i].Enabled)
                {
                    _connections[i].ToNode._inputVal += _outputVal * _connections[i].Weight;
                }
            }
        }

        public Node Clone()
        {
            Node clone = new Node(_innovationNum);
            clone._layer = _layer;
            return clone;
        }

        public bool IsConnectedTo(Node node)
        {
            if (node._layer == _layer)
            {
                return false;
            }

            if (node._layer < _layer)
            {
                return node.IsConnectedTo(this);
            }

            for (int i = 0; i < _connections.Count; i++)
            {
                if (_connections[i].ToNode == node)
                {
                    return true;
                }
            }

            return false;
        }
    }
}