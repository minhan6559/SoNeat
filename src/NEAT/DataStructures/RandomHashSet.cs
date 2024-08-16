using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NEATRex.src.NEAT.DataStructures
{
    public class RandomHashSet<T>
    {
        private readonly HashSet<T> _set;
        private readonly List<T> _data;

        public RandomHashSet()
        {
            _set = new HashSet<T>();
            _data = new List<T>();
        }

        public int Count => _data.Count;
        public List<T> Data => _data;

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public T GetRandom()
        {
            if (_data.Count == 0)
                return default!;

            Random random = new Random();
            return _data[random.Next(_data.Count)];
        }

        public void Add(T item)
        {
            if (_set.Contains(item))
                return;

            _set.Add(item);
            _data.Add(item);
        }

        public void Clear()
        {
            _set.Clear();
            _data.Clear();
        }

        public T GetAt(int index)
        {
            if (index < 0 || index >= _data.Count)
                return default!;

            return _data[index]!;
        }

        public void Remove(T item)
        {
            if (!_set.Contains(item))
                return;

            _set.Remove(item);
            _data.Remove(item);
        }

        public void Remove(int index)
        {
            if (index < 0 || index >= _data.Count)
                return;

            _set.Remove(_data[index]);
            _data.RemoveAt(index);
        }
    }
}