using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.NEAT.Gene;
using SoNeat.src.NEAT.DataStructures;

namespace SoNeat.src.NEAT.Calculation
{
    public class Calculator
    {
        private List<Node> _inputNodes, _outputNodes, _hiddenNodes;

        public Calculator(Genome g)
        {
            _inputNodes = new List<Node>();
            _outputNodes = new List<Node>();
            _hiddenNodes = new List<Node>();

            RandomHashSet<NodeGene> nodes = g.Nodes;
            RandomHashSet<ConnectionGene> conns = g.Connections;

            Dictionary<int, Node> nodeMap = new Dictionary<int, Node>();

            foreach (NodeGene node in nodes.Data)
            {
                Node n = new Node(node.X);
                nodeMap.Add(node.InnovationNum, n);

                if (n.X <= 0.1)
                    _inputNodes.Add(n);
                else if (n.X >= 0.9)
                    _outputNodes.Add(n);
                else
                    _hiddenNodes.Add(n);
            }

            _hiddenNodes.Sort();

            foreach (ConnectionGene conn in conns.Data)
            {
                NodeGene GeneFrom = conn.FromNode;
                NodeGene GeneTo = conn.ToNode;

                Node NodeFrom = nodeMap[GeneFrom.InnovationNum];
                Node NodeTo = nodeMap[GeneTo.InnovationNum];

                Connection c = new Connection(NodeFrom, NodeTo, conn.Weight, conn.Enabled);
                NodeTo.Connections.Add(c);
            }
        }

        public double[] FeedForward(params double[] inputs)
        {
            if (inputs.Length != _inputNodes.Count)
                throw new ArgumentException("Input length does not match the number of input nodes");

            for (int i = 0; i < _inputNodes.Count; i++)
            {
                _inputNodes[i].Output = inputs[i];
            }

            foreach (Node node in _hiddenNodes)
            {
                node.FeedForward();
            }

            double[] outputs = new double[_outputNodes.Count];
            for (int i = 0; i < _outputNodes.Count; i++)
            {
                _outputNodes[i].FeedForward();
                outputs[i] = _outputNodes[i].Output;
            }

            return outputs;
        }
    }
}