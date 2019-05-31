using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.AI.Network;
using Racing.Engine.Entities.World;
using Racing.Engine.GameComponents;
using Racing.Engine.Level;
using Racing.Engine.Resources;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities
{
    class Player : Kinetic
    {
        public PlayerIndex PlayerIndex { get; private set; }
        Input input;

        public bool Dead { get; private set; }
        public int Health { get; private set; }

        Sprite car;
        Sprite roof;

        List<Ray2D> rays;
        float sightLength;
        float fov;
        float startingAngle;
        float angleIncrement;
        float angleIncrementIndex;
        float angle;

        float velocity;
        float terminalVelocity;
        float turningForce;
        float deceleration;

        Timer progress;
        int checkPointIndex;
        bool checkPointReached;

        public NeuralNetwork Brain { get; private set; }
        public double DefaultScore { get; private set; }
        public double Score { get; set; }
        public double Fitness { get; set; }
        double[] inputs;
        Player parent;
        
        int r;
        int g;
        int b;        

        bool playerControlled;

        public Player(float x, float y) : base(x, y, 1, 1, 1)
        {
            PlayerIndex = PlayerIndex.One;
            input = new Input(PlayerIndex);

            sightLength = 50;
            fov = 135 * (float)Math.PI * 2 / 360;
            angleIncrement = 15 * (float)Math.PI * 2 / 360;
            angle = (float)Math.PI / 2;

            MoveSpeed = 15f;
            terminalVelocity = 10;
            turningForce = 2;
            deceleration = 2f;

            car = new Sprite(X, Y, Sprite.Type.Car);
            car.SetCenter(8, 8);
            car.Rotation = angle;
            roof = new Sprite(X, Y, Sprite.Type.Roof);
            roof.SetCenter(8, 8);
            roof.Rotation = angle;

            rays = new List<Ray2D>();

            checkPointIndex = 0;

            CreateRays();

            Brain = new NeuralNetwork(rays.Count + 3, 2, rays.Count + 3 + 2, 3, ActivationFunction.Functions.ReLU);
            inputs = new double[Brain.TotalInputNodes];

            progress = new Timer(3000);

            r = Playfield.RNG.Next(0, 256);
            g = Playfield.RNG.Next(0, 256);
            b = Playfield.RNG.Next(0, 256);
            ObjectColor = new Color(r, g, b);
            roof.SetColor(ObjectColor);
        }

        public Player(Player player) : base(0, 0, 1, 1, 1)
        {
            Brain = new NeuralNetwork(player.Brain);
            ObjectColor = new Color(player.ObjectColor, 255);
            DefaultScore = player.DefaultScore;
        }

        public void SetParent(Player parent)
        {
            this.parent = parent;
        }

        public void Breed(double probability, double standardDeviation)
        {
            Brain = new NeuralNetwork(parent.Brain);
            Brain.Mutate(probability, standardDeviation);

            r = parent.ObjectColor.R + (int)(50 * RandomHelper.Range(-standardDeviation, standardDeviation));
            g = parent.ObjectColor.G + (int)(50 * RandomHelper.Range(-standardDeviation, standardDeviation));
            b = parent.ObjectColor.B + (int)(50 * RandomHelper.Range(-standardDeviation, standardDeviation));
            r = r > 255 ? 255 : r;
            g = g > 255 ? 255 : g;
            b = b > 255 ? 255 : b;
            r = r < 0 ? 0 : r;
            g = g < 0 ? 0 : g;
            b = b < 0 ? 0 : b;
            ObjectColor = new Color(r, g, b);
            roof.SetColor(ObjectColor);

            if (playerControlled)
            {
                ObjectColor = Palette.BloodRed;
                roof.SetColor(ObjectColor);
            }

            Reset();
        }

        public void Reset()
        {
            Dead = false;
            angle = (float)Math.PI / 2;
            car.Rotation = angle;
            roof.Rotation = angle;

            velocity = 0;
            checkPointIndex = 0;

            progress.Reset();
            parent = null;
        }

        public void ActivateInputControls()
        {
            playerControlled = true;
            ObjectColor = Palette.BloodRed;
            roof.SetColor(ObjectColor);        
        }

        private void CreateRays()
        {
            rays.Clear();
            
            startingAngle = angle - fov / 2;
            angleIncrementIndex = 0;

            while (angleIncrementIndex <= fov)
            {
                rays.Add(new Ray2D(X, Y, startingAngle + angleIncrementIndex, sightLength));
                angleIncrementIndex += angleIncrement;
            }
        }

        public new void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);

            car.SetLocation(X, Y);
            roof.SetLocation(X, Y);

            CreateRays();
        }

        public new void SetCenter(float x, float y)
        {
            base.SetCenter(x, y);
            SetLocation(x - Width / 2, y - Height / 2);
        }

        protected override void ApplyForce(GameTime gameTime)
        {

        }

        protected override void Collision()
        {
            checkPointReached = false;

            foreach (Ray2D ray in rays)
            {
                ray.Intersects(Playfield.RaceTrack.Checkpoints[checkPointIndex % Playfield.RaceTrack.Checkpoints.Count]);

                if (ray.CollidedWithBoundary && !checkPointReached)
                {
                    checkPointIndex++;
                    checkPointReached = true;
                }

                foreach (Boundary b in Playfield.RaceTrack.Boundaries)
                {
                    ray.Intersects(b);
                }

                if (!ray.CollidedWithBoundary)
                {
                    foreach (Boundary b in Playfield.RaceTrack.OutOfBounds)
                    {
                        ray.Intersects(b);
                        if (ray.CollidedWithBoundary)
                        {
                            Dead = true;
                            break;
                        }
                    }
                }
            }

            if (checkPointReached)
            {
                progress.Reset();
            }
        }

        private void AIInput(GameTime gameTime)
        {
            if (Brain.Output == null || playerControlled)
                return;

            if (velocity > 0)
            {
                if (Brain.Output[2, 0] > 0.5)
                {
                    angle -= turningForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (Brain.Output[1, 0] > 0.5)
                {
                    angle += turningForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                angle = angle > 2 * (float)Math.PI ? 0 : angle;
                angle = angle < 0 ? (float)Math.PI * 2 : angle;
            }

            if (Brain.Output[0, 0] > 0.5)
            {
                velocity += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity = velocity > terminalVelocity ? terminalVelocity : velocity;
            }
            else
            {
                velocity = velocity > 0 ? velocity - deceleration * (float)gameTime.ElapsedGameTime.TotalSeconds : 0;
            }

            SetLocation(X + velocity * ScaledMoveSpeed * (float)Math.Cos(angle), Y + velocity * ScaledMoveSpeed * (float)Math.Sin(angle));
        }

        private void PlayerInput(GameTime gameTime)
        {
            if (!playerControlled)
                return;

            input.Update(gameTime);

            if (velocity > 0)
            {
                if (input.Pressing(Input.InputType.MovementLeft) || input.Pressing(Input.InputType.AttackLeft))
                {
                    angle -= 2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
                else if (input.Pressing(Input.InputType.MovementRight) || input.Pressing(Input.InputType.AttackRight))
                {
                    angle += 2 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                angle = angle > 2 * (float)Math.PI ? 0 : angle;
                angle = angle < 0 ? (float)Math.PI * 2 : angle;
            }

            if (input.Pressing(Input.InputType.Select))
            {
                velocity += 1 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity = velocity > terminalVelocity ? terminalVelocity : velocity;
            }
            else
            {
                velocity = velocity > 0 ? velocity - deceleration * (float)gameTime.ElapsedGameTime.TotalSeconds : 0;
            }

            SetLocation(X + velocity * ScaledMoveSpeed * (float)Math.Cos(angle), Y + velocity * ScaledMoveSpeed * (float)Math.Sin(angle));
        }

        private void UpdateInput(GameTime gameTime)
        {
            AIInput(gameTime);
            PlayerInput(gameTime);            
        }

        private void UpdateAnimation(GameTime gameTime)
        {
            car.Rotation = angle;
            roof.Rotation = angle;

            if (angle < (float)Math.PI / 4 * 1)
            {
                car.SetFrame(0, 8);
                roof.SetFrame(0, 8);
            }
            else if (angle < (float)Math.PI / 4 * 3)
            {
                car.SetFrame(2, 8);
                roof.SetFrame(2, 8);
            }
            else if (angle < (float)Math.PI / 4 * 5)
            {
                car.SetFrame(4, 8);
                roof.SetFrame(4, 8);
            }
            else if (angle < (float)Math.PI / 4 * 7)
            {
                car.SetFrame(6, 8);
                roof.SetFrame(6, 8);
            }
        }

        private void UpdateNeuralNetwork(GameTime gameTime)
        {
            if (playerControlled)
                return;

            for (int i = 0; i < rays.Count; i++)
            {
                inputs[i] = rays[i].Ray.Magnitude / sightLength;
                inputs[i] = inputs[i] > 1 ? 1 : inputs[i];
            }
           
            inputs[inputs.Length - 3] = (terminalVelocity + velocity * (float)Math.Cos(angle)) / (terminalVelocity * 2);
            inputs[inputs.Length - 2] = (terminalVelocity + velocity * (float)Math.Sin(angle)) / (terminalVelocity * 2);

            inputs[inputs.Length - 1] = angle / (Math.PI * 2) > 1 ? 1 : angle / (Math.PI * 2);

            foreach (double d in inputs)
            {
                if (d > 1)
                {
                    Console.WriteLine(" > 1 !!!");
                }
            }

            Brain.Predict(inputs);
        }

        private void CheckForEndCondition(GameTime gameTime)
        {
            progress.Update(gameTime);
            if (progress.Done)
            {
                Dead = true;
            }

            if (checkPointIndex >= Playfield.RaceTrack.Checkpoints.Count * 3)
            {
                Dead = true;
            }
        }

        public override void Update(GameTime gameTime)
        {
            CalculateScaledSpeed(gameTime);
            UpdateCollisionRectangles();
        
            if (!Dead)
            {
                UpdateNeuralNetwork(gameTime);
                UpdateInput(gameTime);
                Collision();
                CheckForEndCondition(gameTime);
                UpdateAnimation(gameTime);
                UpdateLayerDepth();
            }

            DefaultScore = checkPointIndex;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.DebugMode)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
                {
                    // DrawCollisionRectangles(spriteBatch);
                }
                spriteBatch.End();

                foreach (Ray2D ray in rays)
                {
                    ray.Draw(spriteBatch);
                }
            }
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
            {
                car.Draw(spriteBatch);
                roof.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
