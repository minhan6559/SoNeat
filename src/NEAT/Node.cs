using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.Utils;
using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    [Serializable]
    public class Node
    {
        [JsonProperty]
        private int _innovationNum;
        [JsonProperty]
        private int _layer;
        [JsonProperty]
        private double _inputVal, _outputVal;
        [JsonProperty]
        public double X { get; set; } = 0;
        [JsonProperty]
        public double Y { get; set; } = 0;
        [JsonProperty]
        private List<Connection> _connections;

        [JsonConstructor]
        public Node()
        {
            // _innovationNum = 0;
            // _layer = 0;
            // _inputVal = 0;
            // _outputVal = 0;
            // _connections = new List<Connection>();
        }

        public Node(int innovationNum)
        {
            _innovationNum = innovationNum;
            _layer = 0;
            _inputVal = 0;
            _outputVal = 0;
            _connections = new List<Connection>();
        }

        [JsonIgnore]
        public int InnovationNum
        {
            get => _innovationNum;
            set => _innovationNum = value;
        }

        [JsonIgnore]
        public int Layer
        {
            get => _layer;
            set => _layer = value;
        }

        [JsonIgnore]
        public double InputVal
        {
            get => _inputVal;
            set => _inputVal = value;
        }

        [JsonIgnore]
        public double OutputVal
        {
            get => _outputVal;
            set => _outputVal = value;
        }

        [JsonIgnore]
        public List<Connection> Connections
        {
            get => _connections;
            set => _connections = value;
        }

        public void FeedForward()
        {
            if (_layer != 0)
            {
                _outputVal = Utility.Sigmoid(_inputVal);
            }

            for (int i = 0; i < _connections.Count; i++)
            {
                if (_connections[i].Enabled)
                {
                    _connections[i].ToNode.InputVal += _outputVal * _connections[i].Weight;
                }
            }
        }

        public Node Clone()
        {
            Node clone = new Node(_innovationNum);
            clone.Layer = _layer;
            return clone;
        }

        public bool IsConnectedTo(Node node)
        {
            if (node.Layer == _layer)
            {
                return false;
            }

            if (node.Layer < _layer)
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