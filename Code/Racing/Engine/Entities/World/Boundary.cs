using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Entities.Geometry;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities.World
{
    class Boundary : Entity
    {
        public Line Wall { get; private set; }

        public Boundary(float x1, float y1, float x2, float y2) : base(x1, y1, (int)Math.Abs(x2 - x1), (int)Math.Abs(y2 - y1))
        {
            Wall = new Line(x1, y1, x2, y2, 1, Palette.FleshPink);
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
            {
                Wall.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
