using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT.Gene
{
    public class NodeGene : Gene
    {
        private double _x, _y;

        public NodeGene(int innovation) : base(innovation)
        {
        }

        public NodeGene(int innovation, double x, double y) : base(innovation)
        {
            _x = x;
            _y = y;
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

        public override int GetHashCode()
        {
            return InnovationNum;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not NodeGene)
                return false;

            NodeGene node = (NodeGene)obj;
            return InnovationNum == node.InnovationNum;
        }

        public override Gene Clone()
        {
            return new NodeGene(InnovationNum, _x, _y);
        }
    }
}