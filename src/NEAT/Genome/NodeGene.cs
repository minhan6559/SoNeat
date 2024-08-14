using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NEATRex.src.NEAT.Genome
{
    public class NodeGene : Gene
    {
        private double _x, _y;

        public NodeGene(int innovation) : base(innovation)
        {
        }

        public double X
        {
            get => _x;
            set => _x = value;
        }

        public double Y
        {
            get => _y;
            set => _y = value;
        }

        public override int HashCode => InnovationNum;

        public override bool IsEqual(object other)
        {
            if (other is not NodeGene)
                return false;

            NodeGene node = (NodeGene)other;
            return InnovationNum == node.InnovationNum;
        }

    }
}