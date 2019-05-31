using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.GameComponents;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities.Geometry
{
    class Circle : Entity
    {
        const float INCREMENT = 1 * 2 * (float)Math.PI / 360;
        List<Line> lines;
        public Shape Bounds { get; private set; }
        public float Radius { get; private set; }
        public float LineWidth { get; private set; }

        public Circle(float x, float y, float radius) : this(x, y, radius, radius, Color.White)
        {

        }

        public Circle(float x, float y, float radius, Color color) : this(x, y, radius, radius, color)
        {

        }

        public Circle(float x, float y, float radius, float lineWidth) : this(x, y, radius, lineWidth, Color.White)
        {

        }

        public Circle(float x, float y, float radius, float lineWidth, Color color) : base(x, y, 1, 1)
        {            
            Radius = radius;
            LineWidth = lineWidth;
            ObjectColor = color;
            Bounds = new Shape(X - radius, Y - radius, (int)(radius * 2), (int)(radius * 2), 1, Palette.GrassGreen);

            lines = new List<Line>();

            CreateCircle(X, Y);            
        }

        private void CreateCircle(float x, float y)
        {
            lines.Clear();
            for (float i = 0; i <= Math.PI; i += INCREMENT)
            {
                lines.Add(new Line(x - Radius + CircleX(i), y + CircleY(i), x - Radius + CircleX(i + INCREMENT), y + CircleY(i + INCREMENT), LineWidth, ObjectColor));
            }
        }

        public new void SetLocation(float x, float y)
        {
            base.SetLocation(x, y);
            CreateCircle(X, Y);
        }

        public new void SetCenter(float x, float y)
        {
            SetLocation(x, y);
        }

        public new void SetCollisionRectangle(float x, float y, int width, int height)
        {
            SetLocation(x, y);
        }

        public void SetLineWidth(float lineWidth)
        {
            LineWidth = lineWidth <= Radius ? lineWidth : Radius;
            CreateCircle(X, Y);
        }

        public void SetRadius(float radius)
        {
            Radius = radius;
            CreateCircle(X, Y);
        }

        public new void SetColor(Color color)
        {
            base.SetColor(color);
            CreateCircle(X, Y);
        }

        private float CircleX(float x)
        {
            return (float)((Math.Cos(x)) * Math.Cos(x)) * Radius * 2;
        }

        private float CircleY(float y)
        {
            return (float)((Math.Cos(y)) * Math.Sin(y)) * Radius * 2;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Game1.DebugMode)
            {
                Bounds.Draw(spriteBatch);
            }

            foreach (Line l in lines)
            {
                l.Draw(spriteBatch);
            }
        }
    }
}
