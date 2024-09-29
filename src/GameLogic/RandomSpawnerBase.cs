using System;
using System.Collections.Generic;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    public abstract class RandomSpawnerBase
    {
        protected List<GameObject> _gameObjects;
        protected float _gameSpeed;
        protected double _nextSpawnInterval;
        protected static readonly Random _random = new Random();

        protected RandomSpawnerBase(float initialGameSpeed)
        {
            _gameObjects = new List<GameObject>();
            _gameSpeed = initialGameSpeed;
            _nextSpawnInterval = 0;
        }

        public virtual void Update()
        {
            foreach (var gameObject in _gameObjects)
            {
                gameObject.Update();
            }

            RemoveOffScreenObjects();
            CheckTimer();
        }

        public virtual void UpdateGameSpeed(float gameSpeed)
        {
            _gameSpeed = gameSpeed;
            foreach (var gameObject in _gameObjects)
            {
                gameObject.UpdateGameSpeed(gameSpeed);
            }
        }

        protected virtual void RemoveOffScreenObjects()
        {
            _gameObjects.RemoveAll(obj => obj.IsOffScreen());
        }

        protected abstract void CheckTimer();

        public virtual void Draw()
        {
            foreach (var gameObject in _gameObjects)
            {
                gameObject.Draw();
            }
        }

        protected abstract void SetNextSpawnInterval();
    }
}