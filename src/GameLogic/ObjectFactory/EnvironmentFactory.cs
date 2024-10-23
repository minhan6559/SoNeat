namespace SoNeat.src.GameLogic
{
    // Environment factory class for creating environment objects
    public class EnvironmentFactory : IGameObjectFactory
    {
        private readonly Random _random = new Random();

        public GameObject CreateGameObject(GameObjectType type, float gameSpeed, float xPosition, float yPosition)
        {
            return type switch
            {
                // Environment objects
                GameObjectType.Ground => new Ground(xPosition, yPosition, gameSpeed),
                GameObjectType.Cloud => CreateCloud(xPosition, yPosition),

                _ => throw new ArgumentException($"Unknown game object type: {type}")
            };
        }

        private GameObject CreateCloud(float xPosition, float yPosition)
        {
            // Random cloud speed
            float randomSpeed = (float)(_random.NextDouble() * 1.0f + 3.0f);
            return new Cloud(xPosition, yPosition, randomSpeed);
        }
    }
}