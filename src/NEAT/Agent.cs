using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    [Serializable]
    public class Agent
    {
        [JsonProperty]
        private Genome? _genome;

        [JsonProperty]
        private double _fitness;

        [JsonConstructor]
        public Agent()
        {
            // _genome = new Genome(0, 0, false);
            // _fitness = 0.0;
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

        public Agent Crossover(Agent other)
        {
            Agent child = new Agent(_genome!.CrossOver(other.Genome));
            child.Genome.CreateNetwork();
            return child;
        }

        public void Mutate(List<ConnectionHistory> innovationHistory)
        {
            _genome!.Mutate(innovationHistory);
            _genome.CreateNetwork();
        }

        public Agent Clone()
        {
            Agent child = new Agent(_genome!.Clone());
            child.Genome.CreateNetwork();
            return child;
        }
    }
}