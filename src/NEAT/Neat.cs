using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class Neat
    {
        private int _inputSize, _outputSize;
        private List<Species> _species;
        private List<Agent> _agents;
        private List<ConnectionHistory> _innovationHistory;

        public static int NextConnectionNum = 1000;
        public const double MUTATE_NODE_PROB = 0.02f, MUTATE_CONNECTION_PROB = 0.05f;
        public const double MUTATE_WEIGHT_PROB = 0.8f, MUTATE_WEIGHT_SHIFT_PROB = 0.9f;
        public const double MUTATE_TOGGLE_PROB = 0.001f;

        public const double SURVIVAL_RATE = 0.5f;
        public const int MAX_NOT_IMPROVED_GENERATIONS = 15;

        public Neat(int inputSize, int outputSize, int populationSize)
        {
            _inputSize = inputSize;
            _outputSize = outputSize;
            _species = new List<Species>();
            _agents = new List<Agent>();
            _innovationHistory = new List<ConnectionHistory>();

            for (int i = 0; i < populationSize; i++)
            {
                Agent agent = new Agent(_inputSize, _outputSize);
                agent.Mutate(_innovationHistory);
                _agents.Add(agent);
            }
        }

        public List<Agent> Agents
        {
            get => _agents;
        }

        public void Evolve()
        {
            CreateSpecies();
            SortSpecies();
            KillWeakAgents();
            KillUnfitSpecies();
            KillExtinctSpecies();
            Reproduce();
        }

        private void CreateSpecies()
        {
            foreach (Species species in _species)
            {
                species.Agents.Clear();
            }

            foreach (Agent agent in _agents)
            {
                bool speciesFound = false;

                foreach (Species species in _species)
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
                    _species.Add(newSpecies);
                }
            }
        }

        private void SortSpecies()
        {
            foreach (Species species in _species)
            {
                species.SortAgents();
            }

            _species.Sort((a, b) => b.TopFitness.CompareTo(a.TopFitness));
        }

        private void KillWeakAgents()
        {
            foreach (Species species in _species)
            {
                species.KillWeakAgents();
                species.FitnessSharing();
                species.CalculateFitness();
            }
        }

        private void KillUnfitSpecies()
        {
            for (int i = 2; i < _species.Count; i++)
            {
                if (_species[i].NotImprovedGenerations >= MAX_NOT_IMPROVED_GENERATIONS)
                {
                    _species.RemoveAt(i);
                    i--;
                }
            }
        }

        private void KillExtinctSpecies()
        {
            double averageFitness = CalculateAverageFitnessSum();

            for (int i = 1; i < _species.Count; i++)
            {
                if (_species[i].AverageFitness / averageFitness * _agents.Count < 1)
                {
                    _species.RemoveAt(i);
                    i--;
                }
            }
        }

        private double CalculateAverageFitnessSum()
        {
            double averageFitness = 0.0;
            foreach (Species species in _species)
            {
                averageFitness += species.AverageFitness;
            }

            return averageFitness;
        }

        private void Reproduce()
        {
            double averageFitness = CalculateAverageFitnessSum();
            List<Agent> newAgents = new List<Agent>();

            foreach (Species species in _species)
            {
                int breed = (int)Math.Floor(species.AverageFitness / averageFitness * _agents.Count) - 1;
                newAgents.Add(species.Representative.Clone());

                for (int i = 0; i < breed; i++)
                {
                    newAgents.Add(species.Reproduce(_innovationHistory));
                }
            }

            while (newAgents.Count < _agents.Count)
            {
                Species species = SelectRandomSpecies();
                newAgents.Add(species.Reproduce(_innovationHistory));
            }

            _agents = newAgents;

            foreach (Agent agent in _agents)
            {
                agent.Genome.CreateNetwork();
            }
        }

        private Species SelectRandomSpecies()
        {
            double fitnessSum = 0.0;
            foreach (Species species in _species)
            {
                fitnessSum += species.AverageFitness;
            }

            double randomValue = new Random().NextDouble() * fitnessSum;
            double runningSum = 0.0;

            foreach (Species species in _species)
            {
                runningSum += species.AverageFitness;
                if (runningSum > randomValue)
                {
                    return species;
                }
            }

            return _species[0];
        }

        public void PrintSpecies()
        {
            Console.WriteLine("##########################################");
            foreach (Species s in _species)
            {
                Console.WriteLine(s + "  " + s.TopFitness + " " + s.AverageFitness + "  " + s.Agents.Count);
            }
        }
    }
}