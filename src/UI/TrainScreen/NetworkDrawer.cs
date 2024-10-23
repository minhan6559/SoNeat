using SoNeat.src.NEAT;
using SplashKitSDK;

namespace SoNeat.src.UI.TrainScreen
{
    // Class to draw the neural network
    public class NetworkDrawer
    {
        private string[] _inputLabels, _outputLabels;
        private double _x, _y;
        private double _panelWidth, _panelHeight;

        public NetworkDrawer(string[] inputLabels, string[] outputLabels, double x, double y, double panelWidth, double panelHeight)
        {
            _inputLabels = inputLabels;
            _outputLabels = outputLabels;
            _x = x;
            _y = y;
            _panelWidth = panelWidth;
            _panelHeight = panelHeight;
        }

        // Draw the neural network
        public void Draw(Genome? genome)
        {
            // If genome is null, return
            if (genome == null)
                return;

            // Calculate the node positions
            CalculateNodePositions(genome);

            // Draw the connections and nodes
            foreach (NEAT.Connection connection in genome.Connections)
            {
                DrawConnection(connection);
            }

            foreach (Node node in genome.Nodes)
            {
                DrawNode(node);
            }

            // Draw the labels
            DrawLabels();
        }

        // Calculate the node positions
        private void CalculateNodePositions(Genome g)
        {
            // Create a list of nodes per layer
            List<Node>[] nodesPerLayer = new List<Node>[g.TotalLayers];

            foreach (Node node in g.Nodes)
            {
                if (nodesPerLayer[node.Layer] == null)
                {
                    nodesPerLayer[node.Layer] = new List<Node>();
                }

                nodesPerLayer[node.Layer].Add(node);
            }

            // Input nodes
            for (int i = 0; i < nodesPerLayer[0].Count; i++)
            {
                nodesPerLayer[0][i].X = 0.1;
                nodesPerLayer[0][i].Y = (i + 1) / (double)(nodesPerLayer[0].Count + 1);
            }

            // Output nodes
            for (int i = 0; i < nodesPerLayer[^1].Count; i++)
            {
                nodesPerLayer[^1][i].X = 0.9;
                nodesPerLayer[^1][i].Y = (i + 1) / (double)(nodesPerLayer[^1].Count + 1);
            }

            // Hidden nodes
            for (int i = 1; i < nodesPerLayer.Length - 1; i++)
            {
                double nodeX = (i + 1) / (double)(nodesPerLayer.Length + 1);
                for (int j = 0; j < nodesPerLayer[i].Count; j++)
                {
                    double nodeY = (j + 1) / (double)(nodesPerLayer[i].Count + 1);

                    nodesPerLayer[i][j].X = nodeX;
                    nodesPerLayer[i][j].Y = nodeY;
                }
            }
        }

        // Draw a node
        private void DrawNode(Node node)
        {
            double nodeX = _x + node.X * _panelWidth;
            double nodeY = _y + node.Y * _panelHeight;

            // Draw the node circle and border
            SplashKit.FillCircle(Color.White, nodeX, nodeY, 10);
            SplashKit.DrawCircle(Color.Black, nodeX, nodeY, 10);
        }

        // Draw a connection
        private void DrawConnection(NEAT.Connection connection)
        {
            Node fromNode = connection.FromNode;
            Node toNode = connection.ToNode;

            double fromX = _x + fromNode.X * _panelWidth;
            double fromY = _y + fromNode.Y * _panelHeight;
            double toX = _x + toNode.X * _panelWidth;
            double toY = _y + toNode.Y * _panelHeight;

            Color color;
            // If connection is enabled, draw it blue, otherwise red
            if (connection.Enabled)
            {
                color = Color.RGBAColor(0, 0, 250, Math.Max(0.2, Math.Abs(connection.Weight)) * 255);
            }
            else
            {
                color = Color.RGBAColor(190, 0, 0, 255);
            }

            SplashKit.DrawLine(color, fromX, fromY, toX, toY);
        }

        // Draw the labels
        private void DrawLabels()
        {
            // Draw the input labels
            for (int i = 0; i < _inputLabels.Length; i++)
            {
                double nodeX = 0.1;
                double nodeY = (i + 1) / (double)(_inputLabels.Length + 2);

                double labelX = _x + nodeX * _panelWidth - 11 * _inputLabels[i].Length - 5;
                double labelY = _y + nodeY * _panelHeight - 5;

                SplashKit.DrawText(_inputLabels[i], Color.Black, "MainFont", 10, labelX, labelY);
            }

            // Draw the bias label
            double nodeBiasX = 0.1;
            double nodeBiasY = (_inputLabels.Length + 1) / (double)(_inputLabels.Length + 2);

            double labelBiasX = _x + nodeBiasX * _panelWidth - 11 * "Bias".Length - 10;
            double labelBiasY = _y + nodeBiasY * _panelHeight - 5;

            SplashKit.DrawText("Bias", Color.Black, "MainFont", 10, labelBiasX, labelBiasY);

            for (int i = 0; i < _outputLabels.Length; i++)
            {
                double nodeX = 0.9;
                double nodeY = (i + 1) / (double)(_outputLabels.Length + 1);

                double labelX = _x + nodeX * _panelWidth + 15;
                double labelY = _y + nodeY * _panelHeight - 5;

                SplashKit.DrawText(_outputLabels[i], Color.Black, "MainFont", 10, labelX, labelY);
            }
        }
    }
}