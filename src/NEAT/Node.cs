using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.Utils;
using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    // Node class for the NEAT algorithm
    [Serializable]
    public class Node : IPrototype<Node>
    {
        [JsonProperty]
        private int _innovationNum;
        [JsonProperty]
        private int _layer;
        [JsonProperty]
        private double _inputVal, _outputVal;
        [JsonProperty]
        private double _x, _y;
        [JsonProperty]
        private List<Connection>? _connections;

        [JsonConstructor]
        public Node()
        {
        }

        public Node(int innovationNum)
        {
            _innovationNum = innovationNum;
            _layer = 0;
            _inputVal = 0;
            _outputVal = 0;
            _x = 0;
            _y = 0;
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
        public double X
        {
            get => _x;
            set => _x = value;
        }

        [JsonIgnore]
        public double Y
        {
            get => _y;
            set => _y = value;
        }

        [JsonIgnore]
        public List<Connection> Connections
        {
            get => _connections!;
            set => _connections = value;
        }

        public void FeedForward()
        {
            if (_layer != 0)
            {
                _outputVal = Utility.Sigmoid(_inputVal);
            }

            for (int i = 0; i < _connections!.Count; i++)
            {
                if (_connections[i].Enabled)
                {
                    // Feed forward the connection
                    _connections[i].ToNode.InputVal += _outputVal * _connections[i].Weight;
                }
            }
        }

        // Clone the node
        public Node Clone()
        {
            Node clone = new Node(_innovationNum);
            clone.Layer = _layer;
            return clone;
        }

        // Check if the node is connected to another node
        public bool IsConnectedTo(Node node)
        {
            // If the node is in the same layer, it is not connected
            if (node.Layer == _layer)
            {
                return false;
            }

            // If the node is in a previous layer, check if it is connected to this node
            if (node.Layer < _layer)
            {
                return node.IsConnectedTo(this);
            }

            // Check if the node is connected to the other node
            for (int i = 0; i < _connections!.Count; i++)
            {
                if (_connections[i].ToNode == node)
                {
                    return true;
                }
            }

            // If the node is not connected to the other node, return false
            return false;
        }
    }
}