using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.NEAT;
using SoNeat.src.Utils;

namespace SoNeat.src.GameLogic
{
    // Sonic class for managing Sonic object
    public class Sonic : GameObject
    {
        public const string DEFAULT_FOLDER_PATH = "assets/images/Sonic";
        private bool _isIdle, _isJumping, _isDucking, _isHoldJump, _isDead; // Sonic states
        private float _velocityY, _floorY, _gravity; // Sonic physics
        private int _lifeSpan, _score; // Sonic stats
        private double[] _vision = new double[6]; // Sonic vision
        private Agent? _brain; // Sonic brain

        public Sonic(float x, float y, float floorY, float gameSpeed, string folderPath = DEFAULT_FOLDER_PATH)
                    : base(x, y, 0, gameSpeed, folderPath)
        {
            _isIdle = true;
            _isJumping = false;
            _isDucking = false;
            _isHoldJump = false;
            _isDead = false;

            _velocityY = 0;
            _floorY = floorY;
            _gravity = 1f;

            _lifeSpan = 0;
            _score = 0;

            _brain = null;
        }

        public bool IsIdle { get => _isIdle; set => _isIdle = value; }
        public bool IsDead { get => _isDead; set => _isDead = value; }
        public bool IsJumping => _isJumping;
        public bool IsDucking => _isDucking;

        public Agent? Brain
        {
            get => _brain;
            set => _brain = value;
        }

        public override void Update()
        {
            if (!IsIdle && !IsDead)
                Move();

            Sprite.Update();

            _lifeSpan++;
            if (_lifeSpan % 3 == 0)
            {
                _score++;
            }
        }

        // Move Sonic
        public override void Move()
        {
            if (_isJumping)
            {
                Y -= _velocityY;
                _velocityY -= _gravity;
            }

            if (IsOnGround())
            {
                _velocityY = 0;
                _gravity = 0;
                _isJumping = false;
            }

            if (!_isJumping && !_isDucking && !_isHoldJump)
            {
                Run();
            }

            if (IsOnGround())
                Y = _floorY - CurrentBitmap.Height;
        }

        // Take action based on input
        public void HandleInput()
        {
            if (SplashKit.KeyTyped(KeyCode.DownKey))
            {
                Duck();
            }

            if (SplashKit.KeyUp(KeyCode.DownKey))
            {
                StopDucking();
            }

            if (SplashKit.KeyDown(KeyCode.SpaceKey))
            {
                Jump();
                StopDucking();
                _isHoldJump = true;
            }

            if (SplashKit.KeyUp(KeyCode.SpaceKey))
            {
                _isHoldJump = false;
            }
        }

        // Jump Sonic
        public void Jump()
        {
            if (_isJumping)
                return;

            _velocityY = 22;
            _gravity = 1.2f;
            _isJumping = true;

            if (_isDucking)
            {
                StopDucking();
            }

            PlayAnimation("Jump");
        }

        // Duck Sonic
        public void Duck()
        {
            if (_isDucking)
                return;

            _isDucking = true;
            _gravity = 5;

            PlayAnimation("Duck");
            if (!_isJumping)
                Y = _floorY - CurrentBitmap.Height;
        }

        // Stop ducking Sonic
        public void StopDucking()
        {
            _isDucking = false;
        }

        public void Run()
        {
            PlayAnimation("Run");
        }

        // Sonic sees the obstacles and updates its vision
        public void See(List<Obstacle> obstacles)
        {
            // Reset vision
            for (int i = 0; i < _vision.Length; i++)
            {
                _vision[i] = 0;
            }

            // Find the next enemy
            int nextEnemyIndex = -1;
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (!obstacles[i].HasPassedPlayer(this))
                {
                    nextEnemyIndex = i;
                    break;
                }
            }

            // If no enemy found
            if (nextEnemyIndex == -1)
            {
                // All vision values set to 0
                Array.Fill(_vision, 0);
                return;
            }


            // Distance To Next Enemy
            Obstacle closestEnemy = obstacles[nextEnemyIndex];
            double distanceToNextEnemy = Math.Abs(closestEnemy.X + closestEnemy.CurrentBitmap.Width / 2 - X + CurrentBitmap.Width / 2);

            // Next Enemy Width
            double nextEnemyWidth = closestEnemy.CurrentBitmap.Width;

            // Next Enemy Height
            double nextEnemyHeight = closestEnemy.CurrentBitmap.Height;

            // Sonic Y Position
            _vision[0] = Utility.Normalize(Y, 296, 509, 1, 0);
            _vision[1] = Utility.Normalize(distanceToNextEnemy, 0, 1250, 1, 0);
            _vision[2] = Utility.Normalize(nextEnemyWidth, 0, 150, 0, 1);
            // _vision[2] = Normalize(nextEnemyWidth, 85, 150, 0, 1);
            _vision[3] = Utility.Normalize(nextEnemyHeight, 0, 175, 0, 1);
            // _vision[3] = Normalize(nextEnemyHeight, 50, 175, 0, 1);

            // Bat Y Position
            // Check if next enemy is a bat
            if (closestEnemy is Bat)
            {
                _vision[4] = Utility.Normalize(Math.Abs(closestEnemy.Y + closestEnemy.CurrentBitmap.Height - Y), 0, 123, 0, 1);
                // _vision[4] = Normalize(closestEnemy.Y, 348, 465, 0, 1);
            }
            else
            {
                _vision[4] = 0;
            }

            // Game Speed
            _vision[5] = Utility.Normalize(GameSpeed, 10, 50, 0, 1);
        }

        // Sonic takes action based on its vision
        public void TakeAction()
        {
            // If Sonic has no brain
            if (_brain == null)
                return;

            // Feed forward the vision
            double[] decision = _brain.FeedForward(_vision)!;

            // Find the highest value in the decision array and its index
            double highestValue = decision.Max();

            // If highest value is less than 0.7, Sonic stops ducking
            if (highestValue < 0.7)
            {
                StopDucking();
                return;
            }

            int highestIndex = Array.IndexOf(decision, highestValue);

            switch (highestIndex)
            {
                case 0:
                    Jump();
                    break;
                case 1:
                    Duck();
                    break;
            }
        }

        // Calculate Sonic fitness
        public void CalculateFitness()
        {
            double fitness = _score * _score;

            _brain!.Fitness = fitness;
        }

        // Reset Sonic fitness elements
        public void ResetFitnessElements()
        {
            _score = 0;
            _lifeSpan = 0;
        }

        // Check if Sonic is on the ground
        public bool IsOnGround()
        {
            return Y >= _floorY - CurrentBitmap.Height;
        }
    }
}