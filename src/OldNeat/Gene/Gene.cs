using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT.Gene
{
    public abstract class Gene
    {
        public int InnovationNum { get; set; }

        public Gene(int innovation)
        {
            InnovationNum = innovation;
        }

        public Gene()
        {
        }

        public abstract override bool Equals(object? obj);
        public abstract override int GetHashCode();
        public abstract Gene Clone();
    }
}