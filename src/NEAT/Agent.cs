using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    // Agent class for creating agents
    [Serializable]
    public class Agent : IPrototype<Agent>
    {
        [JsonProperty]
        private Genome? _genome; // Genome of the agent

        [JsonProperty]
        private double _fitness; // Fitness of the agent

        [JsonConstructor]
        public Agent()
        {
        }

        public Agent(Genome genome)
        {
            _genome = genome;
            _fitness = 0.0;
        }

        public Agent(int inputSize, int outputSize)
        {
            _genome = new Genome(inputSize, outputSize, false);
            _fitness = 0.0;
        }

        [JsonIgnore]
        public Genome Genome
        {
            get => _genome!;
        }

        [JsonIgnore]
        public double Fitness
        {
            get => _fitness;
            set => _fitness = value;
        }

        public double[]? FeedForward(params double[] inputs)
        {
            return _genome!.FeedForward(inputs);
        }

        // Create a crossover agent
        public Agent Crossover(Agent other)
        {
            Agent child = new Agent(_genome!.CrossOver(other.Genome));
            child.Genome.CreateNetwork();
            return child;
        }

        // Mutate the agent
        public void Mutate(List<ConnectionHistory> innovationHistory)
        {
            _genome!.Mutate(innovationHistory);
            _genome.CreateNetwork();
        }

        // Clone the agent
        public Agent Clone()
        {
            Agent child = new Agent(_genome!.Clone());
            child.Genome.CreateNetwork();
            return child;
        }
    }
}