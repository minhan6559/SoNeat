using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoNeat.src.NEAT;

namespace SoNeat.src.GameLogic
{
    // Population class for managing Sonic objects
    public class Population
    {
        private Sonic[]? _sonics; // List of Sonic objects
        private int _alives; // Number of alive Sonics

        public Sonic[]? Data => _sonics;
        public int Alives { get => _alives; set => _alives = value; }

        public Population(int populationSize)
        {
            if (populationSize <= 0)
            {
                return;
            }

            // Initialize Sonics
            _sonics = new Sonic[populationSize];

            // Create Sonics
            for (int i = 0; i < populationSize; i++)
            {
                _sonics[i] = new Sonic(52, 509, 634, 10);
                _sonics[i].IsIdle = false;
                _sonics[i].PlayAnimation("Run");
            }

            // Set number of alive Sonics
            _alives = populationSize;
        }

        public void Update(List<Obstacle> obstacles)
        {
            // Update each Sonic
            foreach (Sonic sonic in _sonics!)
            {
                // Check if Sonic is alive
                if (!sonic.IsDead)
                {
                    sonic.See(obstacles); // Sonic looks at the obstacles
                    sonic.TakeAction(); // Sonic takes action based on its vision
                    sonic.Update(); // Sonic updates
                }
            }
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            foreach (Sonic sonic in _sonics!)
            {
                sonic.UpdateGameSpeed(gameSpeed);
            }
        }

        public void Draw()
        {
            foreach (Sonic sonic in _sonics!)
            {
                if (!sonic.IsDead)
                    sonic.Draw();
            }
        }

        // Reset population
        public void Reset()
        {
            for (int i = 0; i < _sonics!.Length; i++)
            {
                _sonics[i].PlayAnimation("Dead");
                _sonics[i].PlayAnimation("Run");
                _sonics[i].IsDead = false;

                _sonics[i].CalculateFitness();
                _sonics[i].ResetFitnessElements();
            }
            _alives = _sonics.Length;
        }

        // Connect Sonics to NEAT agents
        public void LinkBrains(Neat neat)
        {
            if (_sonics!.Length != neat.Agents.Count)
            {
                Console.WriteLine(_sonics.Length + " " + neat.Agents.Count);
                throw new Exception("Population size and NEAT agents size must be the same");
            }

            for (int i = 0; i < _sonics!.Length; i++)
            {
                _sonics[i].Brain = neat.Agents[i];
            }
        }
    }
}