using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using NEATRex.src.NEAT.DataStructures;
using NEATRex.src.NEAT.NeuralEvolution;

namespace NEATRex.src.NEAT.Genome
{
    public class Genome
    {
        private RandomHashSet<ConnectionGene> _connections;
        private RandomHashSet<NodeGene> _nodes;

        private Neat _neat;

        public Genome(Neat neat)
        {
            _connections = new RandomHashSet<ConnectionGene>();
            _nodes = new RandomHashSet<NodeGene>();
            _neat = neat;
        }

        public RandomHashSet<ConnectionGene> Connections => _connections;
        public RandomHashSet<NodeGene> Nodes => _nodes;
        public Neat Neat => _neat;

        // Compute the distance between two genomes.
        // The genome of the class itself must have the highest inno num
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

                excess = _connections.Count - index1;
                weightDiff /= Math.Max(similar, 1);

                double N = Math.Max(_connections.Count, other.Connections.Count);
                if (N < 20)
                    N = 1;

                return _neat.C1 * disjoint / N + _neat.C2 * excess / N + _neat.C3 * weightDiff / N;
            }
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
        }

        public void Mutate()
        {
        }
    }
}