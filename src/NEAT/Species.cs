using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    // Species class for the NEAT algorithm
    [Serializable]
    public class Species
    {
        [JsonProperty]
        private List<Agent>? _agents; // List of agents in the species
        [JsonProperty]
        private Agent? _representative; // Representative agent 
        [JsonProperty]
        private Genome? _benchmarkGenome; // Benchmark genome
        [JsonProperty]
        private double _topFitness, _averageFitness; // Top and average fitness
        [JsonProperty]
        private int _notImprovedGenerations; // Number of generations without improvement

        // Constants
        private const double COMPATIBILITY_THRESHOLD = 3.0;
        private const double EXCESS_COEFFICIENT = 1.0;
        private const double WEIGHT_DIFFERENCE_COEFFICIENT = 0.5;
        private const double SURVIVAL_RATE = 0.5;
        private Random _random = new Random();

        [JsonConstructor]
        public Species()
        {
        }

        public Species(Agent? representative = null)
        {
            _agents = new List<Agent>();
            _representative = new Agent(new Genome(0, 0, false));
            _benchmarkGenome = new Genome(0, 0, false);
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

        [JsonIgnore]
        public List<Agent> Agents => _agents!;

        [JsonIgnore]
        public double TopFitness => _topFitness;

        [JsonIgnore]
        public double AverageFitness => _averageFitness;

        [JsonIgnore]
        public int NotImprovedGenerations => _notImprovedGenerations;

        [JsonIgnore]
        public Agent Representative => _representative!;

        // Check if the genome is in the species
        public bool IsInSpecies(Genome genome)
        {
            // Check if the genome is compatible with the species
            double excessDisjoint = CalculateExcessAndDisjoint(genome, _benchmarkGenome!);
            double averageWeightDiff = CalculateAverageWeightDiff(genome, _benchmarkGenome!);

            // Normalize the genome
            double genomeNormalizer = genome.Connections.Count - 20;
            if (genomeNormalizer < 1)
            {
                genomeNormalizer = 1;
            }

            // Calculate the compatibility
            double compatibility = (EXCESS_COEFFICIENT * excessDisjoint / genomeNormalizer) + (WEIGHT_DIFFERENCE_COEFFICIENT * averageWeightDiff);

            // Return if the genome is compatible
            return compatibility < COMPATIBILITY_THRESHOLD;
        }

        // Calculate the excess and disjoint genes
        private double CalculateExcessAndDisjoint(Genome g1, Genome g2)
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

        // Calculate the average weight difference
        private double CalculateAverageWeightDiff(Genome g1, Genome g2)
        {
            // Check if the genomes have connections
            if (g1.Connections.Count == 0 || g2.Connections.Count == 0)
            {
                return 0;
            }

            double totalWeightDiff = 0;
            double matchingGenes = 0;

            // Loop through all connections in the genomes and calculate the weight difference for matching genes
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

            // If there are no matching genes, return 100 as the weight difference is infinite
            if (matchingGenes == 0)
            {
                return 100;
            }

            // Return the average weight difference
            return totalWeightDiff / matchingGenes;
        }

        // Add an agent to the species
        public void Add(Agent agent)
        {
            _agents!.Add(agent);
        }

        // Sort the agents in the species
        public void SortAgents()
        {
            // Sort descending the agents by fitness
            _agents!.Sort((a, b) => b.Fitness.CompareTo(a.Fitness));

            // If no agents, needs to extinct
            if (_agents.Count == 0)
            {
                _notImprovedGenerations = Neat.MAX_NOT_IMPROVED_GENERATIONS;
                return;
            }

            // Check if the top fitness is improved
            if (_agents[0].Fitness > _topFitness)
            {
                _topFitness = _agents[0].Fitness;
                _benchmarkGenome = _agents[0].Genome.Clone();
                _representative = _agents[0].Clone();
                _notImprovedGenerations = 0;
            }
            else
            {
                // Increment the number of generations without improvement
                _notImprovedGenerations++;
            }
        }

        // Remove the agents with low fitness
        public void KillWeakAgents()
        {
            if (_agents!.Count <= 1)
            {
                _notImprovedGenerations = Neat.MAX_NOT_IMPROVED_GENERATIONS;
                return;
            }

            int survivors = (int)Math.Ceiling(SURVIVAL_RATE * _agents.Count);
            _agents.RemoveRange(survivors, _agents.Count - survivors);
        }

        // Calculate the fitness of the species
        public void CalculateFitness()
        {
            _averageFitness = 0.0;
            foreach (Agent agent in _agents!)
            {
                _averageFitness += agent.Fitness;
            }
            _averageFitness /= _agents.Count;
        }

        // Share the fitness among the agents
        public void FitnessSharing()
        {
            foreach (Agent agent in _agents!)
            {
                agent.Fitness /= _agents.Count;
            }
        }

        // Select a random agent from the species based on fitness, with a higher chance for the fittest agents
        public Agent SelectRandomAgent()
        {
            double totalFitness = 0;
            foreach (Agent agent in _agents!)
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

        // Reproduce the species to create a new agent
        public Agent Reproduce(List<ConnectionHistory> innoHistory)
        {
            Agent child;

            // 25% chance to clone the parent
            if (_random.NextDouble() < 0.25)
            {
                Agent parent = SelectRandomAgent();
                child = parent.Clone();
            }
            // 75% chance to crossover the parents
            else
            {
                Agent parent1 = SelectRandomAgent();
                Agent parent2 = SelectRandomAgent();

                // Make sure the fittest parent is the first one
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