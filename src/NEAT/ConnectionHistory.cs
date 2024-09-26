using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace SoNeat.src.NEAT
{
    public class ConnectionHistory
    {
        private int _fromNodeId, _toNodeId;
        private int _innovationNum;
        private HashSet<int> _innovationNumbersSet;

        public ConnectionHistory(int fromNodeId, int toNodeId, int innovationNum, HashSet<int> innovationNumbers)
        {
            _fromNodeId = fromNodeId;
            _toNodeId = toNodeId;
            _innovationNum = innovationNum;

            // Copy the list
            _innovationNumbersSet = new HashSet<int>(innovationNumbers);
        }

        public int InnovationNum
        {
            get => _innovationNum;
        }

        public bool IsMatching(Genome g, Node fromNode, Node toNode)
        {
            if (g.Connections.Count != _innovationNumbersSet.Count)
                return false;

            if (fromNode.InnovationNum != _fromNodeId || toNode.InnovationNum != _toNodeId)
                return false;

            for (int i = 0; i < g.Connections.Count; i++)
            {
                if (!_innovationNumbersSet.Contains(g.Connections[i].InnovationNum))
                    return false;
            }

            return true;
        }
    }
}