using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;

namespace SoNeat.src.GameLogic
{
    // Base spawner manager using Template Method Pattern
    public abstract class ObjectManagerBase
    {
        private readonly List<GameObject> _gameObjects; // List of game objects
        private float _gameSpeed;
        private readonly ISpawnStrategy _spawnStrategy; // Strategy for spawning objects
        private static readonly Random _random = new Random();
        private double _nextSpawnInterval; // Time until next spawn

        protected List<GameObject> GameObjects => _gameObjects;
        protected double NextSpawnInterval
        {
            get => _nextSpawnInterval;
            set => _nextSpawnInterval = value;
        }
        protected float GameSpeed => _gameSpeed;


        protected ObjectManagerBase(float gameSpeed, ISpawnStrategy spawnStrategy)
        {
            _gameObjects = new List<GameObject>();
            _gameSpeed = gameSpeed;
            _spawnStrategy = spawnStrategy;
            _nextSpawnInterval = 0;
        }

        public virtual void Update()
        {
            UpdateGameObjects();
            RemoveOffScreenObjects();
            CheckSpawnTimer();
        }

        protected virtual void UpdateGameObjects()
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Update();
            }
        }

        public virtual void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;

            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.UpdateGameSpeed(_gameSpeed);
            }
        }

        protected virtual void RemoveOffScreenObjects()
        {
            _gameObjects.RemoveAll(obj => obj.IsOffScreen());
        }

        protected virtual void CheckSpawnTimer()
        {
            if (ShouldSpawn())
            {
                SpawnNewObject();
                // Calculate next spawn interval using strategy
                _nextSpawnInterval = _spawnStrategy.CalculateNextSpawnInterval(_random);
            }
            _nextSpawnInterval++;
        }

        protected abstract bool ShouldSpawn();

        private void SpawnNewObject()
        {
            GameObject newObject = _spawnStrategy.CreateGameObject(_gameSpeed, SplashKit.ScreenWidth());
            _gameObjects.Add(newObject);
        }

        public virtual void Draw()
        {
            foreach (GameObject gameObject in _gameObjects)
            {
                gameObject.Draw();
            }
        }
    }
}