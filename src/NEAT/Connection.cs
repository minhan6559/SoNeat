using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;
using SoNeat.src.Utils;

namespace SoNeat.src.NEAT
{
    [Serializable]
    public class Connection
    {
        [JsonProperty]
        private Node? _fromNode, _toNode;
        [JsonProperty]
        private double _weight;
        [JsonProperty]
        private bool _enabled;
        [JsonProperty]
        private int _innovationNum;
        private static Random _random = new Random();

        [JsonConstructor]
        public Connection()
        {
        }

        public Connection(Node fromNode, Node toNode, double weight, int innovationNum)
        {
            _fromNode = fromNode;
            _toNode = toNode;
            _weight = weight;
            _enabled = true;
            _innovationNum = innovationNum;
        }

        [JsonIgnore]
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        [JsonIgnore]
        public int InnovationNum
        {
            get => _innovationNum;
            set => _innovationNum = value;
        }

        [JsonIgnore]
        public double Weight
        {
            get => _weight;
            set => _weight = value;
        }

        [JsonIgnore]
        public Node FromNode
        {
            get => _fromNode!;
            set => _fromNode = value;
        }

        [JsonIgnore]
        public Node ToNode
        {
            get => _toNode!;
            set => _toNode = value;
        }

        public void MutateWeight()
        {
            if (_random.NextDouble() < Neat.MUTATE_WEIGHT_SHIFT_PROB)
            {
                _weight += Utility.RandomGaussian() / 50.0f;

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

        public Connection Clone(Node fromNode, Node toNode)
        {
            Connection clone = new Connection(fromNode, toNode, _weight, _innovationNum);
            clone.Enabled = _enabled;
            return clone;
        }
    }
}