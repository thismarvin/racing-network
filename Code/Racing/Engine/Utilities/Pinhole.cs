using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Entities.Geometry;
using Racing.Engine.GameComponents;

namespace Racing.Engine.Utilities
{
    class Pinhole : MonoObject
    {
        Circle center;

        float velocity;
        float speed;
        int dir;
        float acceleration;
        float scaledAcceleration;
        bool done;
        int frameIndex;

        Type type;

        public enum Type
        {
            Open, Closed
        }

        public Pinhole(float x, float y, Type type) : base(x, y)
        {
            this.type = type;
            speed = 500;
            acceleration = 100;

            switch (type)
            {
                case Type.Closed:
                    center = new Circle(X, Y, -1000 * Camera.Scale, 1000 * Camera.Scale, Color.Black);
                    dir = 1;
                    break;
                case Type.Open:
                    center = new Circle(X, Y, 1000 * Camera.Scale, 1000 * Camera.Scale, Color.Black);
                    dir = 1;
                    break;
            }
        }

        public new void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            frameIndex = 0;
            switch (type)
            {
                case Type.Closed:
                    center.SetLocation(X, Y);
                    break;
                case Type.Open:
                    center.SetLocation(X, Y);
                    break;
            }
        }

        public void SetSpeed(float speed)
        {
            velocity = velocity < 0 ? -speed : speed;
        }

        public void Update(GameTime gameTime)
        {
            if (done)
                return;

            scaledAcceleration = acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if ((type == Type.Open && center.Radius < 1000 * Camera.Scale + 1000) || (type == Type.Closed && center.Radius < 0))
            {
                velocity = (scaledAcceleration * frameIndex + speed * dir) * (float)gameTime.ElapsedGameTime.TotalSeconds;
                center.SetRadius(center.Radius + (int)velocity);
            }
            else
            {
                center.SetRadius(0);
                done = true;
            }
            frameIndex++;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            center.Draw(spriteBatch);
        }
    }
}
