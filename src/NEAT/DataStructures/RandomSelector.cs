using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NEATRex.src.NEAT.DataStructures
{
    public class RandomSelector<T>
    {
        private readonly List<T> _data;
        private readonly List<double> _scores;
        private double _totalScore;

        public RandomSelector()
        {
            _data = new List<T>();
            _scores = new List<double>();
            _totalScore = 0.0f;
        }

        public void Add(T item, double score)
        {
            _data.Add(item);
            _scores.Add(score);
            _totalScore += score;
        }

        public T GetRandom()
        {
            if (_data.Count == 0)
                return default!;

            Random random = new Random();
            double value = random.NextDouble() * _totalScore;

            for (int i = 0; i < _data.Count; i++)
            {
                value -= _scores[i];
                if (value <= 0)
                    return _data[i];
            }

            return _data[_data.Count - 1];
        }

        public void Reset()
        {
            _data.Clear();
            _scores.Clear();
            _totalScore = 0.0f;
        }
    }
}