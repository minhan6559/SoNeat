using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoNeat.src.GameLogic
{
    // Obstacle manager class for managing obstacle objects
    public class ObstacleManager : ObjectManagerBase
    {
        // List of obstacles
        public List<Obstacle> Obstacles => GameObjects.Cast<Obstacle>().ToList();

        public ObstacleManager(float gameSpeed)
            : base(gameSpeed, new ObstacleSpawnStrategy(new ObstacleFactory()))
        {
            NextSpawnInterval = 50;
        }

        protected override bool ShouldSpawn()
        {
            return NextSpawnInterval > 111 - GameSpeed;
        }

        public void Update(Sonic sonic)
        {
            base.Update();

            // Check for collision with Sonic
            if (IsColliding(sonic))
            {
                sonic.IsDead = true;
            }
        }

        public void Update(Population population)
        {
            base.Update();

            // Check for collision with Sonics
            foreach (Sonic sonic in population.Data!)
            {
                if (!sonic.IsDead && IsColliding(sonic))
                {
                    sonic.IsDead = true;
                    population.Alives--;
                }
            }
        }

        private bool IsColliding(Sonic sonic)
        {
            return Obstacles.Any(obstacle => obstacle.IsColliding(sonic));
        }

        public void Reset()
        {
            GameObjects.Clear();
            NextSpawnInterval = 50;
        }
    }
}