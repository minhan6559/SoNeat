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
        private int _innovationNum;
        private static Random _random = new Random();

        public Connection(Node fromNode, Node toNode, double weight, bool enabled, int innovationNum)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = enabled;
            _innovationNum = innovationNum;
        }

        public Connection(Node fromNode, Node toNode, double weight, int innovationNum) : this(fromNode, toNode, weight, true, innovationNum)
        {
        }

        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        public int InnovationNum
        {
            get => _innovationNum;
            set => _innovationNum = value;
        }

        public double Weight
        {
            get => _weight;
            set => _weight = value;
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

        public void MutateWeight()
        {
            if (_random.NextDouble() < Neat.MUTATE_WEIGHT_SHIFT_PROB)
            {
                _weight += RandomGaussian() / 50.0f;

                if (_weight > 1)
                    _weight = 1;
                else if (_weight < -1)
                    _weight = -1;
            }
            else
            {
                _weight = _random.NextDouble() * 2 - 1;
            }
        }

        private double RandomGaussian()
        {
            double u1 = 1.0 - _random.NextDouble(); // uniform(0,1) random number
            double u2 = 1.0 - _random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);  // standard normal distributed value
            return randStdNormal;
        }

        public Connection Clone(Node fromNode, Node toNode)
        {
            Connection clone = new Connection(fromNode, toNode, _weight, _innovationNum);
            clone.Enabled = _enabled;
            return clone;
        }
    }
}