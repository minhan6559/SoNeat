using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json;
using SoNeat.src.Utils;
using Newtonsoft.Json.Linq;

namespace SoNeat.src.NEAT
{
    [Serializable]
    public class Neat
    {
        [JsonProperty]
        private int _inputSize, _outputSize, _generation, _populationSize;
        [JsonProperty]
        private List<Species>? _species;
        [JsonProperty]
        private List<Agent>? _agents;
        [JsonProperty]
        private List<ConnectionHistory>? _innovationHistory;
        [JsonProperty]
        private Agent? _bestAgent;

        [JsonIgnore]
        public static int NextConnectionNum { get; set; } = 1000;

        public const double MUTATE_NODE_PROB = 0.02f, MUTATE_CONNECTION_PROB = 0.05f;
        public const double MUTATE_WEIGHT_PROB = 0.8f, MUTATE_WEIGHT_SHIFT_PROB = 0.9f;
        public const double MUTATE_TOGGLE_PROB = 0.001f;

        public const double SURVIVAL_RATE = 0.5f;
        public const int MAX_NOT_IMPROVED_GENERATIONS = 15;

        [JsonConstructor]
        public Neat()
        {
            // _inputSize = 0;
            // _outputSize = 0;
            // _species! = new List<Species>();
            // _agents! = new List<Agent>();
            // _innovationHistory! = new List<ConnectionHistory>();
            // _generation = 0;
            // _bestAgent! = new Agent();
        }

        public Neat(int inputSize, int outputSize, int populationSize)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;
            _populationSize = populationSize;
            _species = new List<Species>();
            _agents = new List<Agent>();
            _innovationHistory = new List<ConnectionHistory>();
            _generation = 0;

            for (int i = 0; i < populationSize; i++)
            {
                Agent agent = new Agent(_inputSize, _outputSize);
                agent.Mutate(_innovationHistory!);
                _agents!.Add(agent);
            }

            _bestAgent = _agents![0];
        }

        [JsonIgnore]
        public List<Agent> Agents
        {
            get => _agents!;
        }

        [JsonIgnore]
        public List<Species> Species
        {
            get => _species!;
        }

        [JsonIgnore]
        public Agent BestAgent
        {
            get => _bestAgent!;
        }

        [JsonIgnore]
        public int Generation
        {
            get => _generation;
        }

        public void Evolve()
        {
            CreateSpecies();
            SortSpecies();
            SetBestAgent();
            KillWeakAgents();
            KillUnfitSpecies();
            KillExtinctSpecies();
            Reproduce();
            _generation++;
        }

        private void CreateSpecies()
        {
            foreach (Species species in _species!)
            {
                species.Agents.Clear();
            }

            foreach (Agent agent in _agents!)
            {
                bool speciesFound = false;

                foreach (Species species in _species!)
                {
                    if (species.IsInSpecies(agent.Genome))
                    {
                        species.Add(agent);
                        speciesFound = true;
                        break;
                    }
                }

                if (!speciesFound)
                {
                    Species newSpecies = new Species(agent);
                    _species!.Add(newSpecies);
                }
            }
        }

        private void SortSpecies()
        {
            foreach (Species species in _species!)
            {
                species.SortAgents();
            }

            _species!.Sort((a, b) => b.TopFitness.CompareTo(a.TopFitness));
        }

        private void SetBestAgent()
        {
            // Loop through all agents
            Agent agent = _agents![0];
            agent.Fitness = 0;
            foreach (Agent a in _agents!)
            {
                if (a.Fitness > agent.Fitness)
                {
                    agent = a;
                }
            }
            _bestAgent = agent.Clone();
        }

        private void KillWeakAgents()
        {
            foreach (Species species in _species!)
            {
                species.KillWeakAgents();
                species.FitnessSharing();
                species.CalculateFitness();
            }
        }

        private void KillUnfitSpecies()
        {
            for (int i = 2; i < _species!.Count; i++)
            {
                if (_species![i].NotImprovedGenerations >= MAX_NOT_IMPROVED_GENERATIONS)
                {
                    _species!.RemoveAt(i);
                    i--;
                }
            }
        }

        private void KillExtinctSpecies()
        {
            double averageFitness = CalculateAverageFitnessSum();

            for (int i = 1; i < _species!.Count; i++)
            {
                if (_species![i].AverageFitness / averageFitness * _agents!.Count < 1)
                {
                    _species!.RemoveAt(i);
                    i--;
                }
            }
        }

        private double CalculateAverageFitnessSum()
        {
            return _species!.Sum(s => s.AverageFitness);
        }

        private void Reproduce()
        {
            double averageFitness = CalculateAverageFitnessSum();
            List<Agent> newAgents = new List<Agent>();

            foreach (Species species in _species!)
            {
                int breed = (int)Math.Floor(species.AverageFitness / averageFitness * _agents!.Count) - 1;
                newAgents.Add(species.Representative.Clone());

                for (int i = 0; i < breed; i++)
                {
                    newAgents.Add(species.Reproduce(_innovationHistory!));
                }
            }

            while (newAgents.Count < _populationSize)
            {
                Species species = SelectRandomSpecies();
                newAgents.Add(species.Reproduce(_innovationHistory!));
            }

            _agents = newAgents;

            foreach (Agent agent in _agents!)
            {
                agent.Genome.CreateNetwork();
            }
        }

        private Species SelectRandomSpecies()
        {
            double fitnessSum = _species!.Sum(s => s.AverageFitness);
            double randomValue = new Random().NextDouble() * fitnessSum;
            double runningSum = 0.0;

            foreach (Species species in _species!)
            {
                runningSum += species.AverageFitness;
                if (runningSum > randomValue)
                {
                    return species;
                }
            }

            return _species![0];
        }

        public void PrintSpecies()
        {
            Console.WriteLine("##########################################");
            foreach (Species s in _species!)
            {
                Console.WriteLine($"{s} {s.TopFitness} {s.AverageFitness} {s.Agents.Count}");
            }
        }

        public void SerializeToJson(string filePath)
        {
            filePath = Utility.NormalizePath(filePath);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            JObject jObject = JObject.FromObject(this, JsonSerializer.Create(settings));
            jObject.Add("NextConnectionNum", Neat.NextConnectionNum);

            string jsonString = jObject.ToString();
            File.WriteAllText(filePath, jsonString);
        }

        public static Neat? DeserializeFromJson(string filePath)
        {
            filePath = Utility.NormalizePath(filePath);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            };

            string jsonString = File.ReadAllText(filePath);
            JObject jObject = JObject.Parse(jsonString);

            if (jObject.TryGetValue("NextConnectionNum", out JToken? nextConnectionNumToken))
            {
                Neat.NextConnectionNum = nextConnectionNumToken.Value<int>();
                jObject.Remove("NextConnectionNum");
            }

            return jObject.ToObject<Neat>(JsonSerializer.Create(settings));
        }
    }
}