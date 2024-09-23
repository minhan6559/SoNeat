using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.NEAT.Gene;
using SoNeat.src.NEAT.DataStructures;

namespace SoNeat.src.NEAT.NeuralEvolution
{
    public class Species
    {
        private RandomHashSet<Agent> _agents;
        private Agent _representative; // The agent that represents the species
        private double _fitness; // The average fitness of all agents in the species
        private double _bestFitness; // The best fitness of the species
        private int _totalUnimproveGenerations; // The number of generations the species has not improved

        public Species(Agent representative)
        {
            _agents = new RandomHashSet<Agent>();

            _representative = representative;
            _representative.Species = this;
            _bestFitness = representative.Fitness;
            _totalUnimproveGenerations = 0;
            _agents.Add(_representative);

            _fitness = 0.0f;
        }

        public int Count => _agents.Count;
        public double Fitness => _fitness;
        public double BestFitness => _bestFitness;
        public int TotalUnimproveGenerations => _totalUnimproveGenerations;

        public void Reset()
        {
            _representative = _agents.GetRandom();
            _fitness = 0.0f;

            foreach (Agent agent in _agents.Data)
            {
                agent.Species = null;
            }
            _agents.Clear();

            _representative.Species = this;
            _agents.Add(_representative);
        }

        public bool Add(Agent agent)
        {
            if (agent.DistanceTo(_representative) < Neat.CP)
            {
                agent.Species = this;
                _agents.Add(agent);
                return true;
            }

            return false;
        }

        public void ForceAdd(Agent agent)
        {
            agent.Species = this;
            _agents.Add(agent);
        }

        public void BeExtinct()
        {
            foreach (Agent agent in _agents.Data)
            {
                agent.Species = null;
            }
        }

        public void RemoveWeakAgents(double survivalRate)
        {
            // Reverse sort the agents by fitness
            _agents.Data.Sort((a, b) => b.CompareTo(a));

            if (_agents.Count <= 1)
            {
                _totalUnimproveGenerations = 200; // Extinct species
                return;
            }

            if (_agents.GetAt(0).Fitness > _bestFitness)
            {
                _bestFitness = _agents.GetAt(0).Fitness;
                _totalUnimproveGenerations = 0;
            }
            else
            {
                _totalUnimproveGenerations++;
            }

            int survivors = (int)Math.Ceiling(survivalRate * _agents.Count);
            for (int i = survivors; i < _agents.Count; i++)
            {
                _agents.GetAt(i).Species = null;
                _agents.RemoveAt(i);
            }
        }

        public void CalculateFitness()
        {
            _fitness = 0.0f;
            foreach (Agent agent in _agents.Data)
            {
                _fitness += agent.Fitness;
            }
            _fitness /= _agents.Count;
        }

        public Genome Reproduce()
        {
            Agent parent1 = _agents.GetRandom();
            Agent parent2 = _agents.GetRandom();

            if (parent1.Fitness < parent2.Fitness)
                return parent2.Genome.Crossover(parent1.Genome);

            return parent1.Genome.Crossover(parent2.Genome);
        }
    }
}