using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Entities.Geometry;
using Racing.Engine.GameComponents;
using Racing.Engine.Level;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities.World
{
    class Track : Entity
    {
        public List<Boundary> Boundaries { get; private set; }
        public List<Boundary> OutOfBounds { get; private set; }
        public List<Boundary> Checkpoints { get; private set; }
        public Vector2 Start { get; private set; }

        Spline centerTrack;
        Spline outsideTrack;
        Spline insideTrack;
        Spline outSideOutOfBounds;
        Spline insideOutOfBounds;
        List<Vector2> nodes;   
        Input input;
        
        float trackPadding;
        int increment;    
        bool modifiedTrack;
        bool editMode;                                  

        public Track(float x, float y, float trackPadding) : base(x, y, 1, 1)
        {
            this.trackPadding = trackPadding;
            input = new Input(PlayerIndex.One);
            nodes = new List<Vector2>();
            Boundaries = new List<Boundary>();
            OutOfBounds = new List<Boundary>();
            Checkpoints = new List<Boundary>();

            Reset();
        }

        public void Reset()
        {
            List<Vector2> nodes = new List<Vector2>();
            for (float i = 0; i < (float)Math.PI * 2; i += 30 * 2 * (float)Math.PI / 360)
            {
                nodes.Add(new Vector2(X + 100 * (float)Math.Cos(i), Y + 100 * (float)Math.Sin(i)));
            }

            centerTrack = new Spline(nodes, true, 1, Palette.CoinYellow);
            outsideTrack = new Spline(nodes, true, 3, Color.White);
            insideTrack = new Spline(nodes, true, 3, Color.White);

            outSideOutOfBounds = new Spline(nodes, true, 1, Palette.BloodRed);
            insideOutOfBounds = new Spline(nodes, true, 1, Palette.BloodRed);

            outsideTrack.AddPadding(-trackPadding);
            insideTrack.AddPadding(trackPadding);

            outSideOutOfBounds.AddPadding(-trackPadding - 3);
            insideOutOfBounds.AddPadding(trackPadding + 5);

            Start = centerTrack.Circles[0].Location;

            CreateBoundaries();
            CreateCheckPoints();

            SetEditMode(true);
        }

        public void SetEditMode(bool on)
        {
            editMode = on;
            centerTrack.SetPointVisibilty(editMode);
        }

        private void CreateBoundaries()
        {            
            increment = 2;

            Boundaries.Clear();
            for (int i = 0; i < outsideTrack.Lines.Count - increment; i += increment)
            {
                Boundaries.Add(new Boundary(outsideTrack.Lines[i].Origin.X, outsideTrack.Lines[i].Origin.Y, outsideTrack.Lines[i + increment].EndPoint.X, outsideTrack.Lines[i + increment].EndPoint.Y));
            }
            for (int i = 0; i < insideTrack.Lines.Count - increment; i += increment)
            {
                Boundaries.Add(new Boundary(insideTrack.Lines[i].Origin.X, insideTrack.Lines[i].Origin.Y, insideTrack.Lines[i + increment].EndPoint.X, insideTrack.Lines[i + increment].EndPoint.Y));
            }

            OutOfBounds.Clear();
            for (int i = 0; i < outSideOutOfBounds.Lines.Count - increment; i += increment)
            {
                OutOfBounds.Add(new Boundary(outSideOutOfBounds.Lines[i].Origin.X, outSideOutOfBounds.Lines[i].Origin.Y, outSideOutOfBounds.Lines[i + increment].EndPoint.X, outSideOutOfBounds.Lines[i + increment].EndPoint.Y));
            }
            for (int i = 0; i < insideOutOfBounds.Lines.Count - increment; i += increment)
            {
                OutOfBounds.Add(new Boundary(insideOutOfBounds.Lines[i].Origin.X, insideOutOfBounds.Lines[i].Origin.Y, insideOutOfBounds.Lines[i + increment].EndPoint.X, insideOutOfBounds.Lines[i + increment].EndPoint.Y));
            }
        }

        private void CreateCheckPoints()
        {
            Checkpoints.Clear();

            for (int i = 0; i < centerTrack.Lines.Count * 0.90f; i += 4)
            {
                Checkpoints.Add(new Boundary(outsideTrack.Lines[i].X, outsideTrack.Lines[i].Y, insideTrack.Lines[i].X, insideTrack.Lines[i].Y));
            }
        }

        private void SelectNode()
        {
            for (int i = 0; i < centerTrack.Circles.Count; i++)
            {
                if (!centerTrack.PointSelected && input.LeftClick() && input.DynamicCollisionRectangle.Intersects(centerTrack.Circles[i].Bounds.CollisionRectangle))
                {
                    centerTrack.SelectPoint(i);
                    break;
                }
            }

            if (centerTrack.PointSelected)
            {
                modifiedTrack = centerTrack.MovePointTo(input.DynamicCursorLocation.X, input.DynamicCursorLocation.Y);
            }
        }

        private void ModifyTrack()
        {
            if (modifiedTrack)
            {
                nodes.Clear();
                foreach (Circle c in centerTrack.Circles)
                {
                    nodes.Add(new Vector2(c.X, c.Y));
                }

                outsideTrack.SetPoints(nodes, -trackPadding);
                insideTrack.SetPoints(nodes, trackPadding);
                outSideOutOfBounds.SetPoints(nodes, -trackPadding - 3);
                insideOutOfBounds.SetPoints(nodes, trackPadding + 5);

                CreateBoundaries();
                CreateCheckPoints();

                Start = centerTrack.Circles[0].Location;
            }
        }

        private void UnselectNode()
        {
            if (!input.LeftClick())
            {
                centerTrack.UnselectPoint();
            }
        }

        public void UpdateInput(GameTime gameTime)
        {
            if (!editMode)
                return;

            input.Update(gameTime);

            SelectNode();
            ModifyTrack();
            UnselectNode();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateInput(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            centerTrack.Draw(spriteBatch);
            outsideTrack.Draw(spriteBatch);
            insideTrack.Draw(spriteBatch);
            outSideOutOfBounds.Draw(spriteBatch);
            insideOutOfBounds.Draw(spriteBatch);

            if (Game1.DebugMode)
            {
                foreach (Boundary b in Checkpoints)
                {
                    b.Draw(spriteBatch);
                }

                foreach (Boundary b in Boundaries)
                {
                    b.Draw(spriteBatch);
                }

                foreach (Boundary b in OutOfBounds)
                {
                    b.Draw(spriteBatch);
                }
            }
        }
    }
}
