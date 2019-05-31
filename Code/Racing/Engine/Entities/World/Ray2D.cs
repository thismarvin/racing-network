using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Utilities;
using Racing.Engine.Entities.Geometry;

namespace Racing.Engine.Entities.World
{
    class Ray2D : Entity
    {
        public Line Ray { get; private set; }
        public bool CollidedWithBoundary { get; private set; }

        float angle;
        float length;
        float x1;
        float y1;
        float x2;
        float y2;
        float x3;
        float y3;
        float x4;
        float y4;
        float numerator;
        float denominator;
        float t;
        float u;
        bool intersects;

        public Ray2D(float x, float y, float angle, float length) : base(x, y, 1, 1)
        {
            this.angle = angle;
            this.length = length;
            Ray = new Line(X, Y, X + (float)Math.Cos(angle) * length, Y + (float)Math.Sin(angle) * length);
        }

        public new void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            Ray = new Line(X, Y, X + (float)Math.Cos(angle) * length, Y + (float)Math.Sin(angle) * length);
        }

        public void Intersects(Boundary boundary)
        {
            x1 = Ray.Origin.X;
            y1 = Ray.Origin.Y;
            x2 = Ray.EndPoint.X;
            y2 = Ray.EndPoint.Y;
            x3 = boundary.Wall.Origin.X;
            y3 = boundary.Wall.Origin.Y;
            x4 = boundary.Wall.EndPoint.X;
            y4 = boundary.Wall.EndPoint.Y;

            denominator = (x1 - x2) * (y3 - y4) - (y1 - y2) * (x3 - x4);

            if (denominator != 0)
            {
                numerator = (x1 - x3) * (y3 - y4) - (y1 - y3) * (x3 - x4);
                t = numerator / denominator;
                numerator = (x1 - x2) * (y1 - y3) - (y1 - y2) * (x1 - x3);
                u = -numerator / denominator;
                intersects = 0 <= t && t <= 1 && 0 <= u && u <= 1;
            }

            if (intersects)
            {
                Ray.SetEndPoint(x1 + t * (x2 - x1), y1 + t * (y2 - y1));
                CollidedWithBoundary = true;
            }
            else
            {
                CollidedWithBoundary = false;
            }
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
            {
                Ray.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
