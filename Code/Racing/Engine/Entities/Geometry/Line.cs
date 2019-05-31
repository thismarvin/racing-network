using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities.Geometry
{
    class Line : Entity
    {
        public Vector2 Origin { get; private set; }
        public Vector2 EndPoint { get; private set; }
        public float Magnitude { get; private set; }
        float thickness;
        float rotation;                

        public Line(float x1, float y1, float x2, float y2) : this(x1, y1, x2, y2, 1, Color.White)
        {

        }

        public Line(float x1, float y1, float x2, float y2, float thickness, Color color) : base(x1, y1, 1, 1)
        {            
            Origin = new Vector2(x1, y1);
            EndPoint = new Vector2(x2, y2);           
            this.thickness = thickness;
            ObjectColor = color;

            Setup();
        }

        private void Setup()
        {
            Magnitude = (float)(Math.Sqrt(Math.Pow(Math.Abs(EndPoint.X - Origin.X), 2) + Math.Pow(Math.Abs(EndPoint.Y - Origin.Y), 2)));
            rotation = (float)Math.Atan2(EndPoint.Y - Origin.Y, EndPoint.X - Origin.X);
        }

        public new void SetLocation(float x, float y)
        {
            SetOrigin(x, y);
        }

        public void SetOrigin(float x, float y)
        {
            base.SetLocation(x, y);
            Origin = new Vector2(X, Y);
            Setup();
        }

        public void SetEndPoint(float x, float y)
        {
            EndPoint = new Vector2(x, y);
            Setup();
        }

        public void SetThickness(float thickness)
        {
            this.thickness = thickness;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(ShapeManager.Texture, new Vector2(Origin.X * Camera.Scale, Origin.Y * Camera.Scale), null, ObjectColor, rotation, Vector2.Zero, new Vector2(Magnitude * Camera.Scale, thickness * Camera.Scale), SpriteEffects.None, 0);
        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
