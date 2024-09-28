using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    [Serializable]
    public class ConnectionHistory
    {
        [JsonProperty]
        private int _fromNodeId, _toNodeId, _innovationNum;
        [JsonProperty]
        private HashSet<int> _innovationNumbersSet;

        [JsonConstructor]
        public ConnectionHistory()
        {
            // _fromNodeId = 0;
            // _toNodeId = 0;
            // _innovationNum = 0;
            // _innovationNumbersSet = null;
        }

        public ConnectionHistory(int fromNodeId, int toNodeId, int innovationNum, HashSet<int> innovationNumbers)
        {
            _fromNodeId = fromNodeId;
            _toNodeId = toNodeId;
            _innovationNum = innovationNum;

            // Copy the list
            _innovationNumbersSet = new HashSet<int>(innovationNumbers);
        }

        [JsonIgnore]
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