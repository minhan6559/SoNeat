using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class Species
    {
        private List<Agent> _agents;
        private Agent _representative;
        private Genome _benchmarkGenome;
        private double _topFitness;
        private double _averageFitness;
        private int _notImprovedGenerations;

        private const double COMPATIBILITY_THRESHOLD = 3.0;
        private const double EXCESS_COEFFICIENT = 1.0;
        private const double WEIGHT_DIFFERENCE_COEFFICIENT = 0.5;
        private const double SURVIVAL_RATE = 0.5;
        private Random _random = new Random();

        public Species(Agent? representative = null)
        {
            _agents = new List<Agent>();
            _topFitness = 0.0;
            _averageFitness = 0.0;
            _notImprovedGenerations = 0;

            if (representative != null)
            {
                _agents.Add(representative);
                _topFitness = representative.Fitness;
                _representative = representative.Clone();
                _benchmarkGenome = representative.Genome.Clone();
            }
        }

        public List<Agent> Agents
        {
            get => _agents;
            set => _agents = value;
        }

        public double TopFitness
        {
            get => _topFitness;
            set => _topFitness = value;
        }

        public double AverageFitness
        {
            get => _averageFitness;
            set => _averageFitness = value;
        }

        public int NotImprovedGenerations
        {
            get => _notImprovedGenerations;
            set => _notImprovedGenerations = value;
        }

        public Agent Representative
        {
            get => _representative;
            set => _representative = value;
        }

        public bool IsInSpecies(Genome genome)
        {
            double excessDisjoint = CalculateExcessAndDisjoint(genome, _benchmarkGenome);
            double averageWeightDiff = CalculateAverageWeightDiff(genome, _benchmarkGenome);

            double genomeNormalizer = genome.Connections.Count - 20;
            if (genomeNormalizer < 1)
            {
                genomeNormalizer = 1;
            }

            double compatibility = (EXCESS_COEFFICIENT * excessDisjoint / genomeNormalizer) + (WEIGHT_DIFFERENCE_COEFFICIENT * averageWeightDiff);

            return compatibility < COMPATIBILITY_THRESHOLD;
        }

        public double CalculateExcessAndDisjoint(Genome g1, Genome g2)
        {
            double matchingGenes = 0;
            foreach (Connection conn1 in g1.Connections)
            {
                foreach (Connection conn2 in g2.Connections)
                {
                    if (conn1.InnovationNum == conn2.InnovationNum)
                    {
                        matchingGenes++;
                        break;
                    }
                }
            }

            return g1.Connections.Count + g2.Connections.Count - 2 * matchingGenes;
        }

        public double CalculateAverageWeightDiff(Genome g1, Genome g2)
        {
            if (g1.Connections.Count == 0 || g2.Connections.Count == 0)
            {
                return 0;
            }

            double totalWeightDiff = 0;
            double matchingGenes = 0;

            foreach (Connection conn1 in g1.Connections)
            {
                foreach (Connection conn2 in g2.Connections)
                {
                    if (conn1.InnovationNum == conn2.InnovationNum)
                    {
                        totalWeightDiff += Math.Abs(conn1.Weight - conn2.Weight);
                        matchingGenes++;
                        break;
                    }
                }
            }

            if (matchingGenes == 0)
            {
                return 100;
            }

            return totalWeightDiff / matchingGenes;
        }

        public void Add(Agent agent)
        {
            _agents.Add(agent);
        }

        public void SortAgents()
        {
            _agents.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));

            if (_agents.Count == 0)
            {
                _notImprovedGenerations = Neat.MAX_NOT_IMPROVED_GENERATIONS;
                return;
            }

            if (_agents[0].Fitness > _topFitness)
            {
                _topFitness = _agents[0].Fitness;
                _benchmarkGenome = _agents[0].Genome.Clone();
                _representative = _agents[0].Clone();
                _notImprovedGenerations = 0;
            }
            else
            {
                _notImprovedGenerations++;
            }
        }

        public void KillWeakAgents()
        {
            if (_agents.Count <= 1)
            {
                _notImprovedGenerations = Neat.MAX_NOT_IMPROVED_GENERATIONS;
                return;
            }

            int survivors = (int)Math.Ceiling(SURVIVAL_RATE * _agents.Count);
            _agents.RemoveRange(survivors, _agents.Count - survivors);
        }

        public void CalculateFitness()
        {
            _averageFitness = 0.0;
            foreach (Agent agent in _agents)
            {
                _averageFitness += agent.Fitness;
            }
            _averageFitness /= _agents.Count;
        }

        public void FitnessSharing()
        {
            foreach (Agent agent in _agents)
            {
                agent.Fitness /= _agents.Count;
            }
        }

        public Agent SelectRandomAgent()
        {
            double totalFitness = 0;
            foreach (Agent agent in _agents)
            {
                totalFitness += agent.Fitness;
            }

            double randomFitness = _random.NextDouble() * totalFitness;
            double currentFitness = 0;

            foreach (Agent agent in _agents)
            {
                currentFitness += agent.Fitness;
                if (currentFitness >= randomFitness)
                {
                    return agent;
                }
            }

            return _agents[0];
        }

        public Agent Reproduce(List<ConnectionHistory> innoHistory)
        {
            Agent child;

            if (_random.NextDouble() < 0.25)
            {
                Agent parent = SelectRandomAgent();
                child = parent.Clone();
            }
            else
            {
                Agent parent1 = SelectRandomAgent();
                Agent parent2 = SelectRandomAgent();

                if (parent1.Fitness < parent2.Fitness)
                {
                    Agent temp = parent1;
                    parent1 = parent2;
                    parent2 = temp;
                }

                child = parent1.Crossover(parent2);
            }

            child.Mutate(innoHistory);
            return child;
        }
    }
}