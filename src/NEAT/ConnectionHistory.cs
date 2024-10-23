using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace SoNeat.src.NEAT
{
    // Class to store the connection history
    [Serializable]
    public class ConnectionHistory
    {
        [JsonProperty]
        private int _fromNodeId, _toNodeId, _innovationNum; // From and to node ids and innovation number
        [JsonProperty]
        private HashSet<int>? _innovationNumbersSet; // Innovation numbers set

        [JsonConstructor]
        public ConnectionHistory()
        {
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

        // Check if the genome is matching the connection history
        public bool IsMatching(Genome g, Node fromNode, Node toNode)
        {
            if (g.Connections.Count != _innovationNumbersSet!.Count)
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