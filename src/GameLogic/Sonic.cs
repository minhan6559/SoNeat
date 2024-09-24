using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SplashKitSDK;
using SoNeat.src.NEAT;

namespace SoNeat.src.GameLogic
{
    public class Sonic : GameObject
    {
        public const string DEFAULT_FOLDER_PATH = "assets/images/Sonic";
        private bool _isIdle, _isJumping, _isDucking, _isHoldJump, _isDead;
        private float _velocityY, _floorY, _gravity;
        private double[] _vision = new double[6];

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

            TotalJumps = 0;
            DuckUnderBats = 0;
            JumpOverBats = 0;
            Brain = null;
        }


        public Agent? Brain { get; set; }
        public int TotalJumps { get; set; }
        public int DuckUnderBats { get; set; }
        public int JumpOverBats { get; set; }
        public bool IsIdle { get => _isIdle; set => _isIdle = value; }
        public bool IsDead { get => _isDead; set => _isDead = value; }
        public bool IsJumping => _isJumping;
        public bool IsDucking => _isDucking;
        public double Fitness
        {
            get => Brain!.Fitness;
            set => Brain!.Fitness = value;
        }

        public override void Update()
        {
            if (!IsIdle && !IsDead)
                Move();

            Sprite.Update();
        }

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
                _isHoldJump = true;
            }

            if (SplashKit.KeyUp(KeyCode.SpaceKey))
            {
                _isHoldJump = false;
            }
        }

        public void Jump()
        {
            if (_isJumping)
                return;

            _velocityY = 22;
            _gravity = 1.2f;
            _isJumping = true;
            TotalJumps++;

            if (_isDucking)
            {
                StopDucking();
            }

            Sprite.Play("Jump");
        }

        public void Duck()
        {
            if (_isDucking)
                return;

            _isDucking = true;
            _gravity = 5;

            Sprite.Play("Duck");
            if (!_isJumping)
                Y = _floorY - CurrentBitmap.Height;
        }

        public void StopDucking()
        {
            _isDucking = false;
        }

        public void Run()
        {
            Sprite.Play("Run");
        }

        public void See(List<Obstacle> obstacles)
        {
            for (int i = 0; i < _vision.Length; i++)
            {
                _vision[i] = 0;
            }

            int nextEnemyIndex = -1;
            for (int i = 0; i < obstacles.Count; i++)
            {
                if (!obstacles[i].HasPassedPlayer)
                {
                    nextEnemyIndex = i;
                    break;
                }
            }

            if (nextEnemyIndex == -1)
            {
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
            _vision[0] = Normalize(Y, 296, 509, 1, 0);
            _vision[1] = Normalize(distanceToNextEnemy, 0, 1090, 1, 0);
            _vision[2] = Normalize(nextEnemyWidth, 0, 150, 0, 1);
            // _vision[2] = Normalize(nextEnemyWidth, 85, 150, 0, 1);
            _vision[3] = Normalize(nextEnemyHeight, 0, 175, 0, 1);
            // _vision[3] = Normalize(nextEnemyHeight, 50, 175, 0, 1);

            // Bat Y Position
            // Check if next enemy is a bat
            if (closestEnemy is Bat)
            {
                _vision[4] = Normalize(Math.Abs(closestEnemy.Y + closestEnemy.CurrentBitmap.Height - Y), 0, 123, 0, 1);
                // _vision[4] = Normalize(closestEnemy.Y, 348, 465, 0, 1);
            }
            else
            {
                _vision[4] = 0;
            }

            // Game Speed
            _vision[5] = Normalize(GameSpeed, 10, 50, 0, 1);
        }

        private double Normalize(double value, double min, double max, double newMin, double newMax)
        {
            if (value < min)
                return newMin;
            if (value > max)
                return newMax;
            return newMin + (value - min) * (newMax - newMin) / (max - min);
        }

        public void TakeAction()
        {
            if (Brain == null)
                return;

            double[] decision = Brain.FeedForward(_vision)!;

            // Find the highest value in the decision array and its index
            double highestValue = decision[0];
            int highestIndex = 0;

            if (decision[1] > decision[0])
            {
                highestValue = decision[1];
                highestIndex = 1;
            }

            if (highestValue < 0.7)
            {
                StopDucking();
                return;
            }

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

        public void CalculateFitness(double score)
        {
            double fitness = score;
            fitness -= TotalJumps * 5;
            fitness += DuckUnderBats * 100;
            fitness -= JumpOverBats * 15;

            if (fitness <= 0)
            {
                fitness = 0;
            }
            else
            {
                fitness *= fitness;
            }

            Brain!.Fitness = fitness;
        }

        public bool IsOnGround()
        {
            return Y >= _floorY - CurrentBitmap.Height;
        }
    }
}