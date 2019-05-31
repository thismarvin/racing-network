using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.GameComponents;
using Racing.Engine.Utilities;

namespace Racing.Engine.Entities.Geometry
{
    class Spline : Entity
    {
        List<Vector2> predeterminedPoints;
        List<Vector2> calculatedPoints;
        List<Vector2> gradients;
        Vector2 position;
        Vector2 gradient;
        public List<Line> Lines { get; private set; }
        public List<Circle> Circles { get; private set; }

        public bool PointSelected { get; private set; }        
        bool pointsVisible;
        bool loopAround;
        int selectedIndex;
        float thickness;
        float t;
        float blue;
        float green;
        float red;
        float cyan;
        float increment;
                               
        public Spline(List<Vector2> points) : this(points, false, 1, Color.White)
        {

        }

        public Spline(List<Vector2> points, bool loopAround) : this(points, loopAround, 1, Color.White)
        {

        }

        public Spline(List<Vector2> points, bool loopAround, float thickness, Color color) : base(points[0].X, points[0].Y, 1, 1)
        {
            predeterminedPoints = points;
            this.loopAround = loopAround;
            this.thickness = thickness;
            ObjectColor = color;

            calculatedPoints = new List<Vector2>();
            gradients = new List<Vector2>();
            Lines = new List<Line>();
            Circles = new List<Circle>();

            CreateCurve();
        }

        private void CreateCurve()
        {
            calculatedPoints.Clear();
            gradients.Clear();
            position = Vector2.Zero;

            increment = 0.07f;            

            for (int i = 0; i < predeterminedPoints.Count; i++)
            {
                t = 0;
                while (t <= 1)
                {
                    if (loopAround)
                    {
                        // Catmull-Rom spline
                        blue = -(t * t * t) + 2 * (t * t) - t;
                        green = 3 * (t * t * t) - 5 * (t * t) + 2;
                        red = -3 * (t * t * t) + 4 * (t * t) + t;
                        cyan = t * t * t - t * t;

                        position = new Vector2(
                            0.5f * (predeterminedPoints[i % (predeterminedPoints.Count - 1)].X * blue + predeterminedPoints[(i + 1) % (predeterminedPoints.Count - 1)].X * green + predeterminedPoints[(i + 2) % (predeterminedPoints.Count - 1)].X * red + predeterminedPoints[(i + 3) % (predeterminedPoints.Count - 1)].X * cyan),
                            0.5f * (predeterminedPoints[i % (predeterminedPoints.Count - 1)].Y * blue + predeterminedPoints[(i + 1) % (predeterminedPoints.Count - 1)].Y * green + predeterminedPoints[(i + 2) % (predeterminedPoints.Count - 1)].Y * red + predeterminedPoints[(i + 3) % (predeterminedPoints.Count - 1)].Y * cyan));

                        // Catmull-Rom spline Gradient
                        blue = -3 * (t * t) + 4 * t - 1;
                        green = 9 * (t * t) - 10 * t;
                        red = -9 * (t * t) + 8 * t + 1;
                        cyan = 3 * (t * t) - 2 * t;

                        gradient = new Vector2(
                            0.5f * (predeterminedPoints[i % (predeterminedPoints.Count - 1)].X * blue + predeterminedPoints[(i + 1) % (predeterminedPoints.Count - 1)].X * green + predeterminedPoints[(i + 2) % (predeterminedPoints.Count - 1)].X * red + predeterminedPoints[(i + 3) % (predeterminedPoints.Count - 1)].X * cyan),
                            0.5f * (predeterminedPoints[i % (predeterminedPoints.Count - 1)].Y * blue + predeterminedPoints[(i + 1) % (predeterminedPoints.Count - 1)].Y * green + predeterminedPoints[(i + 2) % (predeterminedPoints.Count - 1)].Y * red + predeterminedPoints[(i + 3) % (predeterminedPoints.Count - 1)].Y * cyan));
                    }
                    else
                    {
                        if (i + 3 >= predeterminedPoints.Count)
                            break;

                        // Catmull-Rom spline
                        blue = -(t * t * t) + 2 * (t * t) - t;
                        green = 3 * (t * t * t) - 5 * (t * t) + 2;
                        red = -3 * (t * t * t) + 4 * (t * t) + t;
                        cyan = t * t * t - t * t;

                        position = new Vector2(
                            0.5f * (predeterminedPoints[i].X * blue + predeterminedPoints[i + 1].X * green + predeterminedPoints[i + 2].X * red + predeterminedPoints[i + 3].X * cyan),
                            0.5f * (predeterminedPoints[i].Y * blue + predeterminedPoints[i + 1].Y * green + predeterminedPoints[i + 2].Y * red + predeterminedPoints[i + 3].Y * cyan));

                        // Catmull-Rom spline Gradient
                        blue = -3 * (t * t) + 4 * t - 1;
                        green = 9 * (t * t) - 10 * t;
                        red = -9 * (t * t) + 8 * t + 1;
                        cyan = 3 * (t * t) - 2 * t;

                        gradient = new Vector2(
                            0.5f * (predeterminedPoints[i].X * blue + predeterminedPoints[i + 1].X * green + predeterminedPoints[i + 2].X * red + predeterminedPoints[i + 3].X * cyan),
                            0.5f * (predeterminedPoints[i].Y * blue + predeterminedPoints[i + 1].Y * green + predeterminedPoints[i + 2].Y * red + predeterminedPoints[i + 3].Y * cyan));
                    }

                    gradient.Normalize();

                    calculatedPoints.Add(position);
                    gradients.Add(gradient);

                    t += increment;
                }
            }

            CreateLines();
            CreateCircles();
        }

        private void CreateLines()
        {
            Lines.Clear();
            for (int i = 1; i < calculatedPoints.Count; i++)
            {
                Lines.Add(new Line(calculatedPoints[i - 1].X, calculatedPoints[i - 1].Y, calculatedPoints[i].X, calculatedPoints[i].Y, thickness, ObjectColor));
            }
        }

        private void CreateCircles()
        {
            Circles.Clear();
            for (int i = 0; i < predeterminedPoints.Count; i++)
            {
                Circles.Add(new Circle(predeterminedPoints[i].X, predeterminedPoints[i].Y, 10, 3, Color.Red));
            }
        }

        public void SetPoints(List<Vector2> points)
        {
            predeterminedPoints = points;
            CreateCurve();
        }

        public void SetPoints(List<Vector2> points, float padding)
        {
            predeterminedPoints = points;
            CreateCurve();
            AddPadding(padding);
        }

        public void AddPadding(float padding)
        {
            for (int i = 0; i < calculatedPoints.Count; i++)
            {
                calculatedPoints[i] = new Vector2(calculatedPoints[i].X + padding * -gradients[i].Y, calculatedPoints[i].Y + padding * gradients[i].X);
            }
           
            CreateLines();
        }

        public void SelectPoint(int index)
        {
            PointSelected = true;
            selectedIndex = index;
        }

        public void UnselectPoint()
        {
            PointSelected = false;
        }

        public bool MovePointTo(float x, float y)
        {
            if (predeterminedPoints[selectedIndex].X != x && predeterminedPoints[selectedIndex].Y != y)
            {
                predeterminedPoints[selectedIndex] = new Vector2(x, y);
                CreateCurve();
                return true;
            }
            return false;
        }

        public void SetPointVisibilty(bool visibility)
        {
            pointsVisible = visibility;
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
            {
                foreach (Line l in Lines)
                {
                    l.Draw(spriteBatch);
                }

                if (Game1.DebugMode || pointsVisible)
                {
                    for (int i = 0; i < Circles.Count; i++)
                    {
                        if (loopAround && i == Circles.Count - 1)
                            break;
                        Circles[i].Draw(spriteBatch);
                    }
                }
            }
            spriteBatch.End();
        }
    }
}
