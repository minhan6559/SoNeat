using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NEATRex.src.NEAT.Gene;
using NEATRex.src.NEAT.DataStructures;

namespace NEATRex.src.NEAT.NeuralEvolution
{
    public class Neat
    {
        public static readonly int MAX_NODES = (int)Math.Pow(2, 20);

        public const double C1 = 1.0f, C2 = 1.0f, C3 = 1.0f;
        public const double CP = 4.0f;

        public const double MUTATE_NODE_PROB = 0.003f, MUTATE_CONNECTION_PROB = 0.01f;
        public const double MUTATE_WEIGHT_SHIFT_PROB = 0.002f, MUTATE_WEIGHT_RANDOM_PROB = 0.002f;
        public const double MUTATE_TOGGLE_PROB = 0.001f;

        public const double SURVIVAL_RATE = 0.8f;

        public const double RANDOM_WEIGHT_STRENGTH = 1.0f, SHIFT_WEIGHT_STRENGTH = 0.3f;

        private Dictionary<ConnectionGene, ConnectionGene> _connectionsMap;
        private RandomHashSet<NodeGene> _nodesHashSet;

        private RandomHashSet<Agent> _agents;
        private RandomHashSet<Species> _species;

        private int _inputSize, _outputSize, _max_agents;

        public Neat(int inputSize, int outputSize, int max_agents)
        {
            _connectionsMap = new Dictionary<ConnectionGene, ConnectionGene>();
            _nodesHashSet = new RandomHashSet<NodeGene>();
            _agents = new RandomHashSet<Agent>();
            _species = new RandomHashSet<Species>();

            Reset(inputSize, outputSize, max_agents);
        }

        public int InputSize => _inputSize;
        public int OutputSize => _outputSize;
        public int MaxAgents => _max_agents;
        public RandomHashSet<Agent> Agents => _agents;

        public void Reset(int inputSize, int outputSize, int max_agents)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;
            _max_agents = max_agents;

            _connectionsMap.Clear();
            _nodesHashSet.Clear();
            _agents.Clear();

            for (int i = 0; i < _inputSize; i++)
            {
                NodeGene node = CreateNode();
                node.X = 0.1;
                node.Y = (i + 1) / (double)(_inputSize + 1);
            }

            for (int i = 0; i < _outputSize; i++)
            {
                NodeGene node = CreateNode();
                node.X = 0.9;
                node.Y = (i + 1) / (double)(_outputSize + 1);
            }

            for (int i = 0; i < max_agents; i++)
            {
                Agent agent = new Agent(CreateEmptyGenome());
                agent.CreateCalculator();
                _agents.Add(agent);
            }
        }

        public Genome CreateEmptyGenome()
        {
            Genome genome = new(this);
            for (int i = 0; i < _inputSize + _outputSize; i++)
                genome.Nodes.Add(GetNodeAt(i + 1));

            return genome;
        }

        public NodeGene CreateNode()
        {
            if (_nodesHashSet.Count >= MAX_NODES)
                return null!;

            NodeGene node = new NodeGene(_nodesHashSet.Count + 1);
            _nodesHashSet.Add(node);

            return node;
        }
        public NodeGene GetNodeAt(int index)
        {
            if (index > 0 || index <= _nodesHashSet.Count)
                return _nodesHashSet.GetAt(index - 1);

            return CreateNode();
        }

        public ConnectionGene GetConnection(NodeGene fromNode, NodeGene toNode)
        {
            ConnectionGene connection = new(fromNode, toNode);
            if (_connectionsMap.ContainsKey(connection))
                connection.InnovationNum = _connectionsMap[connection].InnovationNum;
            else
            {
                _connectionsMap.Add(connection, connection);
                connection.InnovationNum = _connectionsMap.Count + 1;
            }

            return connection;
        }
        public ConnectionGene GetConnection(ConnectionGene gene)
        {
            ConnectionGene? connection = gene.Clone() as ConnectionGene;
            return connection!;
        }
        public void SetReplaceIndex(NodeGene a, NodeGene b, int index)
        {
            _connectionsMap[new ConnectionGene(a, b)].ReplaceIndex = index;
        }
        public int GetReplaceIndex(NodeGene a, NodeGene b)
        {
            ConnectionGene conn = new ConnectionGene(a, b);
            if (_connectionsMap.ContainsKey(conn))
                return _connectionsMap[conn].ReplaceIndex;
            return -1;
        }

        public Agent GetAgentAt(int index)
        {
            return _agents.GetAt(index);
        }

        // Evolution
        private void GenerateSpecies()
        {
            foreach (Species s in _species.Data)
                s.Reset();

            foreach (Agent agent in _agents.Data)
            {
                if (agent.Species != null)
                    continue;

                bool found = false;
                foreach (Species species in _species.Data)
                {
                    if (species.Add(agent))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    Species species = new Species(agent);
                    _species.Add(species);
                }
            }

            foreach (Species s in _species.Data)
                s.CalculateFitness();
        }

        private void RemoveWeakAgents()
        {
            foreach (Species s in _species.Data)
                s.RemoveWeakAgents(SURVIVAL_RATE);
        }

        private void RemoveExtinctSpecies()
        {
            for (int i = _species.Count - 1; i >= 0; i--)
            {
                if (_species.GetAt(i).Count <= 1)
                {
                    _species.GetAt(i).BeExtinct();
                    _species.RemoveAt(i);
                }
            }
        }

        private void Reproduce()
        {
            RandomSelector<Species> selector = new();
            foreach (Species s in _species.Data)
                selector.Add(s, s.Fitness);

            foreach (Agent agent in _agents.Data)
            {
                if (agent.Species == null)
                {
                    Species s = selector.GetRandom();
                    agent.Genome = s.Reproduce();
                    s.ForceAdd(agent);
                }
            }
        }

        private void Mutate()
        {
            foreach (Agent agent in _agents.Data)
            {
                agent.Mutate();
            }
        }

        public void Evolve()
        {
            GenerateSpecies();
            RemoveWeakAgents();
            RemoveExtinctSpecies();
            Reproduce();
            Mutate();

            foreach (Agent agent in _agents.Data)
            {
                agent.CreateCalculator();
            }
        }

        public void PrintSpecies()
        {
            Console.WriteLine("##########################################");
            foreach (Species s in _species.Data)
            {
                Console.WriteLine(s + "  " + s.Fitness + "  " + s.Count);
            }
        }
    }
}