using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SoNeat.src.NEAT;

namespace SoNeat.src.GameLogic
{
    public class Population
    {
        private readonly Sonic[]? _sonics;
        private int _generation;
        private int _bestSonicIndex;
        private int _alives;
        private double _bestFitness;

        public Sonic[]? Data => _sonics;
        public int Generation => _generation;
        public int BestSonicIndex => _bestSonicIndex;
        public Genome BestBrain => _sonics![_bestSonicIndex].Brain!.Genome;
        public int Alives { get => _alives; set => _alives = value; }
        public double BestFitness => _bestFitness;

        public Population(int populationSize)
        {
            _sonics = new Sonic[populationSize];

            for (int i = 0; i < populationSize; i++)
            {
                _sonics[i] = new Sonic(52, 509, 634, 10);
                _sonics[i].IsIdle = false;
                _sonics[i].Sprite.Play("Run");
            }

            _generation = 1;
            _bestSonicIndex = 0;
            _bestFitness = 0;
            _alives = populationSize;
        }

        public void Update()
        {
            foreach (Sonic sonic in _sonics!)
            {
                sonic.Update();
            }
        }

        public void Update(List<Obstacle> obstacles, double score)
        {
            foreach (Sonic sonic in _sonics!)
            {
                if (!sonic.IsDead)
                {
                    sonic.See(obstacles);
                    sonic.TakeAction();
                    sonic.Update();
                }
                else
                {
                    if (sonic.IsDead)
                    {
                        sonic.CalculateFitness(score);
                    }
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

        public void Reset()
        {
            double lastBestFitness = -1;
            int bestIndex = 0;

            for (int i = 0; i < _sonics!.Length; i++)
            {
                _sonics[i].Sprite.Play("Dead");
                _sonics[i].Sprite.Play("Run");
                _sonics[i].IsDead = false;

                if (_sonics[i].Fitness > lastBestFitness)
                {
                    lastBestFitness = _sonics[i].Fitness;
                    bestIndex = i;
                }
            }

            _bestSonicIndex = bestIndex;
            _bestFitness = _sonics[bestIndex].Fitness;

            _generation++;
            _alives = _sonics.Length;
        }

        public void LinkBrains(Neat neat)
        {
            if (_sonics!.Length != neat.Agents.Count)
                throw new Exception("Population size and NEAT agents size must be the same");

            for (int i = 0; i < _sonics!.Length; i++)
            {
                _sonics[i].Brain = neat.Agents[i];
            }
        }
    }
}