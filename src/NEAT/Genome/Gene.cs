using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NEATRex.src.NEAT.Genome
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

        public abstract bool IsEqual(object other);
        public abstract int HashCode { get; }
    }
}