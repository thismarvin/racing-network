using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities.Geometry
{
    class Shape : Entity
    {
        List<Rectangle> rectangles;
        int lineWidth;

        public Shape(float x, float y, int width, int height, Color color) : this(x, y, width, height, 0, color)
        {

        }

        public Shape(float x, float y, int width, int height, int lineWidth, Color color) : base(x, y, width, height)
        {
            this.lineWidth = lineWidth;
            ObjectColor = color;

            rectangles = new List<Rectangle>();

            Setup();
        }

        private void Setup()
        {
            rectangles.Clear();
            if (lineWidth == 0)
            {
                rectangles.Add(new Rectangle((int)ScaledLocation.X, (int)ScaledLocation.Y, Width * Camera.Scale, Height * Camera.Scale));
            }
            else
            {
                rectangles.Add(new Rectangle((int)ScaledLocation.X, (int)ScaledLocation.Y, Width * Camera.Scale, lineWidth * Camera.Scale));
                rectangles.Add(new Rectangle((int)ScaledLocation.X, (int)ScaledLocation.Y + (Height - lineWidth) * Camera.Scale, Width * Camera.Scale, lineWidth * Camera.Scale));
                rectangles.Add(new Rectangle((int)ScaledLocation.X, (int)ScaledLocation.Y + lineWidth * Camera.Scale, lineWidth * Camera.Scale, Height * Camera.Scale - lineWidth * 2 * Camera.Scale));
                rectangles.Add(new Rectangle((int)ScaledLocation.X + Width * Camera.Scale - lineWidth * Camera.Scale, (int)ScaledLocation.Y + lineWidth * Camera.Scale, lineWidth * Camera.Scale, Height * Camera.Scale - lineWidth * 2 * Camera.Scale));
            }
        }

        public new void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            Setup();
        }

        public new void SetCenter(float x, float y)
        {
            base.SetCenter(x, y);
            Setup();
        }

        public new void SetWidth(int width)
        {
            base.SetWidth(width);
            Setup();
        }

        public new void SetHeight(int height)
        {
            base.SetHeight(height);
            Setup();
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (Rectangle R in rectangles)
            {
                spriteBatch.Draw(ShapeManager.Texture, R, ObjectColor);
            }
        }
    }
}
