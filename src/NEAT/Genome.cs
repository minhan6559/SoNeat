using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    [Serializable]
    public class Genome
    {
        [JsonProperty]
        private List<Node> _nodes;
        [JsonProperty]
        private List<Connection> _connections;
        [JsonProperty]
        private int _inputSize, _outputSize;
        [JsonProperty]
        private int _totalLayers;
        [JsonProperty]
        private int _nextNodeIndex, _biasNodeIndex;
        [JsonProperty]
        private List<Node> _networkNodes;

        private static Random _random = new Random();

        [JsonConstructor]
        public Genome()
        {
            _nodes = new List<Node>();
            _connections = new List<Connection>();
            _inputSize = 0;
            _outputSize = 0;
            _totalLayers = 0;
            _nextNodeIndex = 0;
            _biasNodeIndex = -1;
            _networkNodes = new List<Node>();
        }

        public Genome(int inputSize, int outputSize, bool isCrossOver)
        {
            _nodes = new List<Node>();
            _connections = new List<Connection>();

            _inputSize = inputSize;
            _outputSize = outputSize;
            _totalLayers = 2;
            _nextNodeIndex = 0;
            _biasNodeIndex = -1;

            _networkNodes = new List<Node>();

            if (isCrossOver)
                return;

            CreateNodes();
        }

        [JsonIgnore]
        public List<Node> Nodes
        {
            get => _nodes;
            set => _nodes = value;
        }

        [JsonIgnore]
        public List<Connection> Connections
        {
            get => _connections;
            set => _connections = value;
        }

        [JsonIgnore]
        public int TotalLayers
        {
            get => _totalLayers;
            set => _totalLayers = value;
        }

        [JsonIgnore]
        public int NextNodeIndex
        {
            get => _nextNodeIndex;
            set => _nextNodeIndex = value;
        }

        [JsonIgnore]
        public int BiasNodeIndex
        {
            get => _biasNodeIndex;
            set => _biasNodeIndex = value;
        }

        private void CreateNodes()
        {
            for (int i = 0; i < _inputSize; i++)
            {
                _nodes.Add(new Node(i));
                _nextNodeIndex++;
                _nodes[i].Layer = 0;
            }

            for (int i = 0; i < _outputSize; i++)
            {
                _nodes.Add(new Node(i + _inputSize));
                _nextNodeIndex++;
                _nodes[i + _inputSize].Layer = 1;
            }

            _nodes.Add(new Node(_nextNodeIndex));
            _biasNodeIndex = _nextNodeIndex;
            _nextNodeIndex++;
            _nodes[_biasNodeIndex].Layer = 0;
        }

        public void ConnectNodes()
        {
            foreach (Node node in _nodes)
            {
                node.Connections.Clear();
            }

            foreach (Connection connection in _connections)
            {
                connection.FromNode.Connections.Add(connection);
            }
        }

        public Node? GetNodeByInnovationNum(int innovationNum)
        {
            foreach (Node node in _nodes)
            {
                if (node.InnovationNum == innovationNum)
                    return node;
            }

            return null;
        }

        public void CreateNetwork()
        {
            ConnectNodes();
            _networkNodes.Clear();

            for (int i = 0; i < _totalLayers; i++)
            {
                foreach (Node node in _nodes)
                {
                    if (node.Layer == i)
                    {
                        _networkNodes.Add(node);
                    }
                }
            }
        }

        public double[] FeedForward(params double[] inputs)
        {
            for (int i = 0; i < _inputSize; i++)
            {
                _nodes[i].OutputVal = inputs[i];
            }

            _nodes[_biasNodeIndex].OutputVal = 1;

            foreach (Node node in _networkNodes)
            {
                node.FeedForward();
            }

            double[] outputs = new double[_outputSize];
            for (int i = 0; i < _outputSize; i++)
            {
                outputs[i] = _nodes[i + _inputSize].OutputVal;
            }

            foreach (Node node in _networkNodes)
            {
                node.InputVal = 0;
            }

            return outputs;
        }

        private bool IsFullyConnected()
        {
            int maxConnections = 0;
            int[] nodesPerLayer = new int[_totalLayers];

            for (int i = 0; i < _totalLayers; i++)
            {
                nodesPerLayer[i] = 0;
            }

            foreach (Node node in _nodes)
            {
                nodesPerLayer[node.Layer]++;
            }

            for (int i = 0; i < _totalLayers - 1; i++)
            {
                int totalNodesPrevious = 0;
                for (int j = i + 1; j < _totalLayers; j++)
                {
                    totalNodesPrevious += nodesPerLayer[j];
                }

                maxConnections += nodesPerLayer[i] * totalNodesPrevious;
            }

            return maxConnections <= _connections.Count;
        }

        public void AddConnection(List<ConnectionHistory> innoHistory)
        {
            if (IsFullyConnected())
                return;

            int fromNodeIndex = _random.Next(_nodes.Count);
            int toNodeIndex = _random.Next(_nodes.Count);

            while (CannotConnectNodes(fromNodeIndex, toNodeIndex))
            {
                fromNodeIndex = _random.Next(_nodes.Count);
                toNodeIndex = _random.Next(_nodes.Count);
            }

            if (_nodes[fromNodeIndex].Layer > _nodes[toNodeIndex].Layer)
            {
                int temp = fromNodeIndex;
                fromNodeIndex = toNodeIndex;
                toNodeIndex = temp;
            }

            int connInnovationNum = GetInnovationNum(innoHistory, _nodes[fromNodeIndex], _nodes[toNodeIndex]);

            _connections.Add(new Connection(_nodes[fromNodeIndex], _nodes[toNodeIndex], _random.NextDouble() * 2 - 1, connInnovationNum));

            ConnectNodes();
        }

        public void AddNode(List<ConnectionHistory> innoHistory)
        {
            if (_connections.Count == 0)
            {
                AddConnection(innoHistory);
                return;
            }

            int randomConnectionIndex;
            do
            {
                randomConnectionIndex = _random.Next(_connections.Count);
            } while (_connections.Count != 1 && _connections[randomConnectionIndex].FromNode == _nodes[_biasNodeIndex]);

            Connection conn = _connections[randomConnectionIndex];
            conn.Enabled = false;

            Node midNode = new Node(_nextNodeIndex);
            _nextNodeIndex++;
            _nodes.Add(midNode);
            midNode.Layer = _nodes[conn.FromNode.InnovationNum].Layer + 1;

            _connections.Add(new Connection(conn.FromNode, midNode, 1, GetInnovationNum(innoHistory, conn.FromNode, midNode)));

            _connections.Add(new Connection(midNode, conn.ToNode, conn.Weight, GetInnovationNum(innoHistory, midNode, conn.ToNode)));

            _connections.Add(new Connection(_nodes[_biasNodeIndex], midNode, 0, GetInnovationNum(innoHistory, _nodes[_biasNodeIndex], midNode)));

            if (midNode.Layer == conn.ToNode.Layer)
            {
                foreach (Node node in _nodes)
                {
                    if (node.Layer >= midNode.Layer && node != midNode)
                    {
                        node.Layer++;
                    }
                }

                _totalLayers++;
            }

            ConnectNodes();
        }

        private bool CannotConnectNodes(int fromNodeIndex, int toNodeIndex)
        {
            if (_nodes[fromNodeIndex].Layer == _nodes[toNodeIndex].Layer)
                return true;

            if (_nodes[fromNodeIndex].IsConnectedTo(_nodes[toNodeIndex]))
                return true;

            return false;
        }

        public int GetInnovationNum(List<ConnectionHistory> innoHistory, Node fromNode, Node toNode)
        {
            bool isNew = true;
            int connInnovationNum = Neat.NextConnectionNum;

            foreach (ConnectionHistory history in innoHistory)
            {
                if (history.IsMatching(this, fromNode, toNode))
                {
                    isNew = false;
                    connInnovationNum = history.InnovationNum;
                    break;
                }
            }

            if (isNew)
            {
                HashSet<int> innovationNumbers = new HashSet<int>();
                foreach (Connection connection in _connections)
                {
                    innovationNumbers.Add(connection.InnovationNum);
                }

                innoHistory.Add(new ConnectionHistory(fromNode.InnovationNum, toNode.InnovationNum, connInnovationNum, innovationNumbers));
                Neat.NextConnectionNum++;
            }

            return connInnovationNum;
        }

        public void Mutate(List<ConnectionHistory> innoHistory)
        {
            if (_connections.Count == 0)
            {
                AddConnection(innoHistory);
                return;
            }

            if (_random.NextDouble() < Neat.MUTATE_WEIGHT_PROB)
            {
                foreach (Connection connection in _connections)
                {
                    connection.MutateWeight();
                }
            }

            if (_random.NextDouble() < Neat.MUTATE_CONNECTION_PROB)
            {
                AddConnection(innoHistory);
            }

            if (_random.NextDouble() < Neat.MUTATE_NODE_PROB)
            {
                AddNode(innoHistory);
            }
        }

        public Genome CrossOver(Genome other)
        {
            Genome child = new Genome(_inputSize, _outputSize, true);
            child.Nodes.Clear();
            child.Connections.Clear();
            child.TotalLayers = _totalLayers;
            child.NextNodeIndex = _nextNodeIndex;
            child.BiasNodeIndex = _biasNodeIndex;

            List<Connection> childConns = new();
            List<bool> enables = new();

            foreach (Connection conn in _connections)
            {
                bool isEnabled = true;

                int matchingConnectionIndex = GetMatchingConnection(other, conn.InnovationNum);
                if (matchingConnectionIndex == -1)
                {
                    childConns.Add(conn);
                    isEnabled = conn.Enabled;
                }
                else
                {
                    if (!conn.Enabled || !other.Connections[matchingConnectionIndex].Enabled)
                    {
                        if (_random.NextDouble() < 0.75)
                        {
                            isEnabled = false;
                        }
                    }

                    if (_random.NextDouble() < 0.5)
                    {
                        childConns.Add(conn);
                    }
                    else
                    {
                        childConns.Add(other.Connections[matchingConnectionIndex]);
                    }
                }

                enables.Add(isEnabled);
            }

            foreach (Node node in _nodes)
            {
                child.Nodes.Add(node.Clone());
            }

            for (int i = 0; i < childConns.Count; i++)
            {
                Connection conn = childConns[i];
                child.Connections.Add(
                    conn.Clone(
                        child.GetNodeByInnovationNum(conn.FromNode.InnovationNum)!,
                        child.GetNodeByInnovationNum(conn.ToNode.InnovationNum)!
                    )
                );
                child.Connections[i].Enabled = enables[i];
            }

            child.ConnectNodes();
            return child;
        }

        public int GetMatchingConnection(Genome other, int innoNum)
        {
            for (int i = 0; i < other.Connections.Count; i++)
            {
                if (other.Connections[i].InnovationNum == innoNum)
                    return i;
            }

            return -1;
        }

        public Genome Clone()
        {
            Genome clone = new Genome(_inputSize, _outputSize, true);

            foreach (Node node in _nodes)
            {
                clone.Nodes.Add(node.Clone());
            }

            foreach (Connection conn in _connections)
            {
                clone.Connections.Add(
                    conn.Clone(
                        clone.GetNodeByInnovationNum(conn.FromNode.InnovationNum)!,
                        clone.GetNodeByInnovationNum(conn.ToNode.InnovationNum)!
                    )
                );
            }

            clone.TotalLayers = _totalLayers;
            clone.NextNodeIndex = _nextNodeIndex;
            clone.BiasNodeIndex = _biasNodeIndex;
            clone.ConnectNodes();

            return clone;
        }
    }
}