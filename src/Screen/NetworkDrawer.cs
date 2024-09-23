using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SoNeat.src.NEAT.Gene;
using SplashKitSDK;

namespace SoNeat.src.Screen
{
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

        public void Draw(Genome genome)
        {
            foreach (ConnectionGene connection in genome.Connections.Data)
            {
                DrawConnection(connection);
            }

            foreach (NodeGene node in genome.Nodes.Data)
            {
                DrawNode(node, 10);
            }

            for (int i = 0; i < _inputLabels.Length; i++)
            {
                double nodeX = 0.1;
                double nodeY = (i + 1) / (double)(_inputLabels.Length + 1);

                double labelX = _x + nodeX * _panelWidth - 11 * _inputLabels[i].Length - 5;
                double labelY = _y + nodeY * _panelHeight - 5;

                SplashKit.DrawText(_inputLabels[i], Color.Black, "MainFont", 10, labelX, labelY);
            }

            for (int i = 0; i < _outputLabels.Length; i++)
            {
                double nodeX = 0.9;
                double nodeY = (i + 1) / (double)(_outputLabels.Length + 1);

                double labelX = _x + nodeX * _panelWidth + 15;
                double labelY = _y + nodeY * _panelHeight - 5;

                SplashKit.DrawText(_outputLabels[i], Color.Black, "MainFont", 10, labelX, labelY);
            }
        }

        private void DrawNode(NodeGene node, double radius)
        {
            double x = _x + node.X * _panelWidth;
            double y = _y + node.Y * _panelHeight;

            SplashKit.FillCircle(Color.White, x, y, radius);
            SplashKit.DrawCircle(Color.Black, x, y, radius);

            // SplashKit.DrawText(node.InnovationNum.ToString(), Color.Black, x - 5, y - 5);

        }

        private void DrawConnection(ConnectionGene connection)
        {
            NodeGene fromNode = connection.FromNode;
            NodeGene toNode = connection.ToNode;

            double fromX = _x + fromNode.X * _panelWidth;
            double fromY = _y + fromNode.Y * _panelHeight;
            double toX = _x + toNode.X * _panelWidth;
            double toY = _y + toNode.Y * _panelHeight;

            Color color;
            if (connection.Enabled)
            {
                color = Color.RGBAColor(0, 0, 250, Math.Max(0.5, Math.Abs(connection.Weight)) * 255);
            }
            else
            {
                color = Color.RGBAColor(190, 0, 0, 255);
            }

            SplashKit.DrawLine(color, fromX, fromY, toX, toY);
        }
    }
}