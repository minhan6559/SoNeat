using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    // Genome class for creating genomes
    [Serializable]
    public class Genome : IPrototype<Genome>
    {
        [JsonProperty]
        private List<Node>? _nodes; // Nodes of the genome
        [JsonProperty]
        private List<Connection>? _connections; // Connections of the genome
        [JsonProperty]
        private int _inputSize, _outputSize;
        [JsonProperty]
        private int _totalLayers;
        [JsonProperty]
        private int _nextNodeIndex, _biasNodeIndex;
        [JsonProperty]
        private List<Node>? _networkNodes; // Network nodes of the genome

        private static Random _random = new Random();

        [JsonConstructor]
        public Genome()
        {
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
            get => _nodes!;
            set => _nodes = value;
        }

        [JsonIgnore]
        public List<Connection> Connections
        {
            get => _connections!;
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

        // Create nodes
        private void CreateNodes()
        {
            // Input nodes
            for (int i = 0; i < _inputSize; i++)
            {
                _nodes!.Add(new Node(i));
                _nextNodeIndex++;
                _nodes![i].Layer = 0;
            }

            // Output nodes
            for (int i = 0; i < _outputSize; i++)
            {
                _nodes!.Add(new Node(i + _inputSize));
                _nextNodeIndex++;
                _nodes![i + _inputSize].Layer = 1;
            }

            // Bias node
            _nodes!.Add(new Node(_nextNodeIndex));
            _biasNodeIndex = _nextNodeIndex;
            _nextNodeIndex++;
            _nodes![_biasNodeIndex].Layer = 0;
        }

        // Connect nodes in the genome 
        public void ConnectNodes()
        {
            // Clear connections
            foreach (Node node in _nodes!)
            {
                node.Connections.Clear();
            }

            // Add connections
            foreach (Connection connection in _connections!)
            {
                connection.FromNode.Connections.Add(connection);
            }
        }

        // Get node by innovation number
        public Node? GetNodeByInnovationNum(int innovationNum)
        {
            // Find the node by innovation number
            foreach (Node node in _nodes!)
            {
                if (node.InnovationNum == innovationNum)
                    return node;
            }

            return null;
        }

        // Create network
        public void CreateNetwork()
        {
            ConnectNodes();
            _networkNodes!.Clear();

            for (int i = 0; i < _totalLayers; i++)
            {
                foreach (Node node in _nodes!)
                {
                    if (node.Layer == i)
                    {
                        _networkNodes.Add(node);
                    }
                }
            }
        }

        // Feed forward
        public double[] FeedForward(params double[] inputs)
        {
            // Set input values
            for (int i = 0; i < _inputSize; i++)
            {
                _nodes![i].OutputVal = inputs[i];
            }

            // Set bias node output
            _nodes![_biasNodeIndex].OutputVal = 1;

            // Feed forward
            foreach (Node node in _networkNodes!)
            {
                node.FeedForward();
            }

            // Get outputs
            double[] outputs = new double[_outputSize];
            for (int i = 0; i < _outputSize; i++)
            {
                outputs[i] = _nodes![i + _inputSize].OutputVal;
            }

            // Reset input values
            foreach (Node node in _networkNodes)
            {
                node.InputVal = 0;
            }

            return outputs;
        }

        // Check if the genome is fully connected
        private bool IsFullyConnected()
        {
            // Calculate the maximum connections
            int maxConnections = 0;
            int[] nodesPerLayer = new int[_totalLayers];

            // Initialize nodes per layer
            for (int i = 0; i < _totalLayers; i++)
            {
                nodesPerLayer[i] = 0;
            }

            // Count nodes per layer
            foreach (Node node in _nodes!)
            {
                nodesPerLayer[node.Layer]++;
            }

            // Calculate maximum connections
            for (int i = 0; i < _totalLayers - 1; i++)
            {
                int totalNodesPrevious = 0;
                for (int j = i + 1; j < _totalLayers; j++)
                {
                    totalNodesPrevious += nodesPerLayer[j];
                }

                maxConnections += nodesPerLayer[i] * totalNodesPrevious;
            }

            // Check if the genome is fully connected
            return maxConnections <= _connections!.Count;
        }

        // Add connection to the genome 
        public void AddConnection(List<ConnectionHistory> innoHistory)
        {
            // Check if the genome is fully connected
            if (IsFullyConnected())
                return;

            // Get random nodes
            int fromNodeIndex = _random.Next(_nodes!.Count);
            int toNodeIndex = _random.Next(_nodes!.Count);

            // Check if the nodes cannot be connected
            while (CannotConnectNodes(fromNodeIndex, toNodeIndex))
            {
                fromNodeIndex = _random.Next(_nodes!.Count);
                toNodeIndex = _random.Next(_nodes!.Count);
            }

            // Swap nodes if needed
            if (_nodes![fromNodeIndex].Layer > _nodes![toNodeIndex].Layer)
            {
                int temp = fromNodeIndex;
                fromNodeIndex = toNodeIndex;
                toNodeIndex = temp;
            }

            // Get innovation number
            int connInnovationNum = GetInnovationNum(innoHistory, _nodes![fromNodeIndex], _nodes![toNodeIndex]);

            // Add connection
            _connections!.Add(new Connection(_nodes![fromNodeIndex], _nodes![toNodeIndex], _random.NextDouble() * 2 - 1, connInnovationNum));

            ConnectNodes();
        }

        // Add node to the genome
        public void AddNode(List<ConnectionHistory> innoHistory)
        {
            // Check if the genome has connections
            if (_connections!.Count == 0)
            {
                AddConnection(innoHistory);
                return;
            }

            // Get random connection
            int randomConnectionIndex;
            do
            {
                randomConnectionIndex = _random.Next(_connections!.Count);
            } while (_connections!.Count != 1 && _connections![randomConnectionIndex].FromNode == _nodes![_biasNodeIndex]);

            // Disable the connection
            Connection conn = _connections![randomConnectionIndex];
            conn.Enabled = false;

            // Add node
            Node midNode = new Node(_nextNodeIndex);
            _nextNodeIndex++;
            _nodes!.Add(midNode);
            midNode.Layer = _nodes![conn.FromNode.InnovationNum].Layer + 1;

            // Add connections
            _connections!.Add(new Connection(conn.FromNode, midNode, 1, GetInnovationNum(innoHistory, conn.FromNode, midNode)));

            _connections!.Add(new Connection(midNode, conn.ToNode, conn.Weight, GetInnovationNum(innoHistory, midNode, conn.ToNode)));

            _connections!.Add(new Connection(_nodes![_biasNodeIndex], midNode, 0, GetInnovationNum(innoHistory, _nodes![_biasNodeIndex], midNode)));

            // Increase layers
            if (midNode.Layer == conn.ToNode.Layer)
            {
                foreach (Node node in _nodes!)
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

        // Check if the nodes cannot be connected
        private bool CannotConnectNodes(int fromNodeIndex, int toNodeIndex)
        {
            if (_nodes![fromNodeIndex].Layer == _nodes![toNodeIndex].Layer)
                return true;

            if (_nodes![fromNodeIndex].IsConnectedTo(_nodes![toNodeIndex]))
                return true;

            return false;
        }

        // Get innovation number of the connection
        public int GetInnovationNum(List<ConnectionHistory> innoHistory, Node fromNode, Node toNode)
        {
            bool isNew = true;
            int connInnovationNum = Neat.NextConnectionNum;

            // Check if the connection is new
            foreach (ConnectionHistory history in innoHistory)
            {
                if (history.IsMatching(this, fromNode, toNode))
                {
                    isNew = false;
                    connInnovationNum = history.InnovationNum;
                    break;
                }
            }

            // Add connection history if new
            if (isNew)
            {
                HashSet<int> innovationNumbers = new HashSet<int>();
                foreach (Connection connection in _connections!)
                {
                    innovationNumbers.Add(connection.InnovationNum);
                }

                innoHistory.Add(new ConnectionHistory(fromNode.InnovationNum, toNode.InnovationNum, connInnovationNum, innovationNumbers));
                Neat.NextConnectionNum++;
            }

            return connInnovationNum;
        }

        // Mutate the genome
        public void Mutate(List<ConnectionHistory> innoHistory)
        {
            //  Mutate the genome
            if (_connections!.Count == 0)
            {
                AddConnection(innoHistory);
                return;
            }

            // Mutate the weight
            if (_random.NextDouble() < Neat.MUTATE_WEIGHT_PROB)
            {
                foreach (Connection connection in _connections!)
                {
                    connection.MutateWeight();
                }
            }

            // Mutate the connection
            if (_random.NextDouble() < Neat.MUTATE_CONNECTION_PROB)
            {
                AddConnection(innoHistory);
            }

            // Mutate the node
            if (_random.NextDouble() < Neat.MUTATE_NODE_PROB)
            {
                AddNode(innoHistory);
            }
        }

        // Crossover the genome
        public Genome CrossOver(Genome other)
        {
            // Create child genome
            Genome child = new Genome(_inputSize, _outputSize, true);
            child.Nodes.Clear();
            child.Connections.Clear();
            child.TotalLayers = _totalLayers;
            child.NextNodeIndex = _nextNodeIndex;
            child.BiasNodeIndex = _biasNodeIndex;

            // Get matching connections
            List<Connection> childConns = new List<Connection>();
            List<bool> enables = new List<bool>();

            // Add connections
            foreach (Connection conn in _connections!)
            {
                bool isEnabled = true;

                // Get matching connection
                int matchingConnectionIndex = GetMatchingConnection(other, conn.InnovationNum);

                // Add connection to the child genome 
                if (matchingConnectionIndex == -1)
                {
                    childConns.Add(conn);
                    isEnabled = conn.Enabled;
                }
                else
                {
                    // Disable connection if not enabled
                    if (!conn.Enabled || !other.Connections[matchingConnectionIndex].Enabled)
                    {
                        // Disable connection with 75% probability
                        if (_random.NextDouble() < 0.75)
                        {
                            isEnabled = false;
                        }
                    }

                    // Get random connection from the parents 
                    if (_random.NextDouble() < 0.5)
                    {
                        childConns.Add(conn);
                    }
                    else
                    {
                        childConns.Add(other.Connections[matchingConnectionIndex]);
                    }
                }

                // Add connection enable
                enables.Add(isEnabled);
            }

            // Add nodes
            foreach (Node node in _nodes!)
            {
                child.Nodes.Add(node.Clone());
            }

            // Add connections to the child genome
            for (int i = 0; i < childConns.Count; i++)
            {
                Connection conn = childConns[i];
                child.Connections.Add(
                    conn.Clone(
                        child.GetNodeByInnovationNum(conn.FromNode.InnovationNum)!,
                        child.GetNodeByInnovationNum(conn.ToNode.InnovationNum)!
                    )
                );

                // Enable connection if needed
                child.Connections[i].Enabled = enables[i];
            }

            // Connect nodes
            child.ConnectNodes();
            return child;
        }

        // Get matching connection from the other genome 
        public int GetMatchingConnection(Genome other, int innoNum)
        {
            for (int i = 0; i < other.Connections.Count; i++)
            {
                if (other.Connections[i].InnovationNum == innoNum)
                    return i;
            }

            return -1;
        }

        // Clone the genome
        public Genome Clone()
        {
            // Create clone genome
            Genome clone = new Genome(_inputSize, _outputSize, true);

            // Add nodes
            foreach (Node node in _nodes!)
            {
                clone.Nodes.Add(node.Clone());
            }

            // Add connections
            foreach (Connection conn in _connections!)
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