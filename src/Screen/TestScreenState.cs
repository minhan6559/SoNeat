using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SoNeat.src.GameLogic;
using SplashKitSDK;

namespace SoNeat.src.Screen
{
    public class TestScreenState : IScreenState
    {
        private Ground? _ground;
        private Sonic? _sonic;
        private float _gameSpeed;
        private List<Obstacle>? _obstacles;

        public void EnterState()
        {
            _gameSpeed = 10;
            _ground = new Ground(0, 634, _gameSpeed);
            _sonic = new Sonic(52, 509, _ground.Y, _gameSpeed);
            _obstacles = new List<Obstacle>()
            {
                ObstacleFactory.CreateObstacle(ObstacleType.Crab, 1080, 560, _gameSpeed)
            };
        }

        public void Update()
        {
            // Handle game logic, player input, and updates
            _sonic!.Update();
            _ground!.Update();

            for (int i = 0; i < _obstacles!.Count; i++)
            {
                _obstacles[i].Update();
                if (_obstacles[i].IsOffScreen())
                {
                    _obstacles.RemoveAt(i);
                    i--;
                }
            }
        }

        public void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;
            _sonic!.GameSpeed = gameSpeed;
            _ground!.GameSpeed = gameSpeed;
            foreach (Obstacle obstacle in _obstacles!)
            {
                obstacle.GameSpeed = gameSpeed;
            }
        }

        public void Draw()
        {
            // Draw the game elements to the screen
            _sonic!.Draw();
            _ground!.Draw();
            foreach (Obstacle obstacle in _obstacles!)
            {
                obstacle.Draw();
            }
        }

        public void ExitState()
        {
            // Clean up the game screen
            Console.WriteLine("Exiting Game Screen State");
        }
    }
}