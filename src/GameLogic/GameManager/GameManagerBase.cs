using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;

namespace SoNeat.src.GameLogic
{
    // Base spawner using Template Method Pattern
    public abstract class GameManagerBase
    {
        protected readonly List<GameObject> gameObjects;
        protected float gameSpeed;
        protected readonly ISpawnStrategy spawnStrategy;
        protected static readonly Random random = new Random();
        protected double nextSpawnInterval;

        protected GameManagerBase(float gameSpeed, ISpawnStrategy spawnStrategy)
        {
            this.gameObjects = new List<GameObject>();
            this.gameSpeed = gameSpeed;
            this.spawnStrategy = spawnStrategy;
            this.nextSpawnInterval = 0;
        }

        public virtual void Update()
        {
            UpdateGameObjects();
            RemoveOffScreenObjects();
            CheckSpawnTimer();
        }

        protected virtual void UpdateGameObjects()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Update();
            }
        }

        public virtual void UpdateGameSpeed(float gameSpeed)
        {
            this.gameSpeed = gameSpeed;

            foreach (GameObject gameObject in gameObjects)
            {
                gameObject.UpdateGameSpeed(gameSpeed);
            }
        }

        protected virtual void RemoveOffScreenObjects()
        {
            gameObjects.RemoveAll(obj => obj.IsOffScreen());
        }

        protected virtual void CheckSpawnTimer()
        {
            if (ShouldSpawn())
            {
                SpawnNewObject();
                nextSpawnInterval = spawnStrategy.CalculateNextSpawnInterval(random);
            }
            nextSpawnInterval++;
        }

        protected abstract bool ShouldSpawn();

        protected virtual void SpawnNewObject()
        {
            var newObject = spawnStrategy.CreateGameObject(gameSpeed, SplashKit.ScreenWidth());
            gameObjects.Add(newObject);
        }

        public virtual void Draw()
        {
            foreach (var gameObject in gameObjects)
            {
                gameObject.Draw();
            }
        }
    }
}