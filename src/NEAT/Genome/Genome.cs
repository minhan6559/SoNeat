using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using NEATRex.src.NEAT.DataStructures;
using NEATRex.src.NEAT.NeuralEvolution;
using NEATRex.src.NEAT.Calculation;

namespace NEATRex.src.NEAT.Genome
{
    public class Genome
    {
        private RandomHashSet<ConnectionGene> _connections;
        private RandomHashSet<NodeGene> _nodes;
        private Neat _neat;
        private Calculator? _calculator;

        public Genome(Neat neat)
        {
            _connections = new RandomHashSet<ConnectionGene>();
            _nodes = new RandomHashSet<NodeGene>();
            _neat = neat;
            _calculator = null;
        }

        public RandomHashSet<ConnectionGene> Connections => _connections;
        public RandomHashSet<NodeGene> Nodes => _nodes;
        public Neat Neat => _neat;

        // Compute the distance between two genomes.
        // The genome of the class itself must have the higher inno num
        public double DistanceTo(Genome other)
        {
            int highestInno1 = _connections.Count > 0 ? _connections.GetAt(_connections.Count - 1).InnovationNum : 0;
            int highestInno2 = other.Connections.Count > 0 ? other.Connections.GetAt(other.Connections.Count - 1).InnovationNum : 0;

            if (highestInno1 < highestInno2)
                return other.DistanceTo(this);

            int index1 = 0, index2 = 0;
            int disjoint = 0, excess = 0, similar = 0;
            double weightDiff = 0.0f;

            while (index1 < _connections.Count && index2 < other.Connections.Count)
            {
                ConnectionGene gene1 = _connections.GetAt(index1);
                ConnectionGene gene2 = other.Connections.GetAt(index2);

                int innovation1 = gene1.InnovationNum;
                int innovation2 = gene2.InnovationNum;


                if (innovation1 == innovation2) // Similar genes
                {
                    similar++;
                    index1++;
                    index2++;

                    weightDiff += Math.Abs(gene1.Weight - gene2.Weight);
                }
                else if (innovation1 < innovation2) // Disjoint gene of A
                {
                    disjoint++;
                    index1++;
                }
                else // Disjoint gene of B
                {
                    disjoint++;
                    index2++;
                }
            }

            excess = _connections.Count - index1;
            weightDiff /= Math.Max(similar, 1);

            double N = Math.Max(_connections.Count, other.Connections.Count);
            if (N < 20)
                N = 1;

            return Neat.C1 * disjoint / N + Neat.C2 * excess / N + Neat.C3 * weightDiff / N;
        }

        // Genome A should have the higher score
        // Choose random similar genes from A and B
        // Choose disjoint genes from A, but not excess genes from B
        // Choose excess genes from A
        public Genome Crossover(Genome other)
        {
            Genome g = _neat.CreateEmptyGenome();
            int index1 = 0, index2 = 0;

            while (index1 < _connections.Count && index2 < other.Connections.Count)
            {
                ConnectionGene gene1 = _connections.GetAt(index1);
                ConnectionGene gene2 = other.Connections.GetAt(index2);

                int innovation1 = gene1.InnovationNum;
                int innovation2 = gene2.InnovationNum;

                if (innovation1 == innovation2)
                {
                    Random rnd = new Random();
                    if (rnd.NextDouble() < 0.5)
                        g.Connections.Add(_neat.GetConnection(gene1));
                    else
                        g.Connections.Add(_neat.GetConnection(gene2));

                    index1++;
                    index2++;
                }
                else if (innovation1 < innovation2)
                {
                    g.Connections.Add(_neat.GetConnection(gene1));
                    index1++;
                }
                else
                {
                    index2++;
                }
            }

            while (index1 < _connections.Count)
            {
                ConnectionGene gene = _connections.GetAt(index1);
                g.Connections.Add(_neat.GetConnection(gene));
                index1++;
            }

            foreach (ConnectionGene gene in other.Connections.Data)
            {
                g.Nodes.Add(gene.FromNode);
                g.Nodes.Add(gene.ToNode);
            }

            return g;
        }

        public void Mutate()
        {
            Random rnd = new Random();
            if (rnd.NextDouble() < Neat.MUTATE_NODE_PROB)
                MutateNode();
            if (rnd.NextDouble() < Neat.MUTATE_CONNECTION_PROB)
                MutateConnection();
            if (rnd.NextDouble() < Neat.MUTATE_WEIGHT_RANDOM_PROB)
                MutateWeightRandom();
            if (rnd.NextDouble() < Neat.MUTATE_WEIGHT_SHIFT_PROB)
                MutateWeightShift();
            if (rnd.NextDouble() < Neat.MUTATE_TOGGLE_PROB)
                MutateToggle();
        }

        public void MutateNode()
        {
            Random rnd = new Random();
            ConnectionGene conn = _connections.GetRandom();
            if (conn == null)
                return;

            NodeGene a = conn.FromNode;
            NodeGene b = conn.ToNode;

            NodeGene mid = _neat.CreateNode();
            mid.X = (a.X + b.X) / 2;
            mid.Y = (a.Y + b.Y) / 2;

            ConnectionGene conn1 = _neat.GetConnection(a, mid);
            ConnectionGene conn2 = _neat.GetConnection(mid, b);

            conn1.Weight = 1.0;
            conn2.Weight = conn.Weight;
            conn2.Enabled = conn.Enabled;

            _connections.Remove(conn);
            _connections.Add(conn1);
            _connections.Add(conn2);

            _nodes.Add(mid);
        }

        public void MutateConnection()
        {
            for (int i = 0; i < 88; i++)
            {
                NodeGene a = _nodes.GetRandom();
                NodeGene b = _nodes.GetRandom();

                if (a == null || b == null)
                    return;

                if (a.X == b.X)
                    continue;

                ConnectionGene conn;
                if (a.X < b.X)
                    conn = new(a, b);
                else
                    conn = new(b, a);

                if (_connections.Contains(conn))
                    continue;

                conn = _neat.GetConnection(conn.FromNode, conn.ToNode);
                Random rnd = new Random();
                conn.Weight = (rnd.NextDouble() * 2 - 1) * Neat.RANDOM_WEIGHT_STRENGTH;

                _connections.AddSortedAscending(conn, a => a.InnovationNum);
                break;
            }
        }

        public void MutateWeightRandom()
        {
            ConnectionGene conn = _connections.GetRandom();

            if (conn == null)
                return;

            Random rnd = new Random();
            conn.Weight = (rnd.NextDouble() * 2 - 1) * Neat.RANDOM_WEIGHT_STRENGTH;
        }

        public void MutateWeightShift()
        {
            ConnectionGene conn = _connections.GetRandom();

            if (conn == null)
                return;

            Random rnd = new Random();
            conn.Weight += (rnd.NextDouble() * 2 - 1) * Neat.SHIFT_WEIGHT_STRENGTH;
        }

        public void MutateToggle()
        {
            ConnectionGene conn = _connections.GetRandom();

            if (conn == null)
                return;

            conn.Enabled = !conn.Enabled;
        }

        public void CreateCalculator()
        {
            _calculator = new Calculator(this);
        }

        public double[]? FeedForward(params double[] inputs)
        {
            if (_calculator != null)
                return _calculator.FeedForward(inputs);
            return null;
        }
    }
}