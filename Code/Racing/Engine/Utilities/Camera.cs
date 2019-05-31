﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Racing.Engine.GameComponents;

namespace Racing.Engine.Utilities
{
    static class Camera
    {
        public static float Zoom { get; set; }
        public static Rectangle ScreenBounds { get; private set; }
        public static Matrix Transform { get; private set; }
        public static Vector2 TopLeft;

        public static int Scale { get; private set; }

        public static void Initialize()
        {
            Reset(ScreenManager.DefaultWindowWidth, ScreenManager.DefaultWindowHeight);
        }

        public static void Reset(int windowWidth, int windowHeight)
        {
            Scale = 3;
            int longSide = 480 * Scale;
            int shortSide = 270 * Scale;

            switch (Game1.GameOrientation)
            {
                case Game1.Orientation.Landscape:
                    Zoom = (float)windowHeight / shortSide;
                    // Check if letterboxing is required.
                    if (longSide * Zoom > windowWidth)
                    {
                        Zoom = (float)windowWidth / longSide;
                    }
                    else if (longSide * Zoom < windowWidth)
                    {
                        // Disable letterboxing if WideScreenSupport is enabled.
                        if (ScreenManager.WideScreenSupport)
                        {
                            longSide = (int)((windowWidth - longSide * Zoom) / Zoom) + longSide;
                        }
                    }
                    ScreenBounds = new Rectangle(0, 0, longSide / Scale, shortSide / Scale);
                    break;

                case Game1.Orientation.Portrait:
                    Zoom = (float)windowWidth / shortSide;
                    // Check if letterboxing is required.
                    if (longSide * Zoom > windowHeight)
                    {
                        Zoom = (float)windowHeight / longSide;
                    }
                    ScreenBounds = new Rectangle(0, 0, shortSide / Scale, longSide / Scale);
                    break;
            }
        }

        public static void Update()
        {
            Input();
            FinalizeMatrix();
        }

        public static void Update(Vector2 topLeft, float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            TopLeft = topLeft * Scale;

            Input();
            StayWithinBounds(minWidth, maxWidth, minHeight, maxHeight);
            FinalizeMatrix();
        }

        private static void Input()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.OemPlus))
            {
                Zoom += 0.01f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.OemMinus))
            {
                Zoom -= 0.01f;
            }
        }

        private static void StayWithinBounds(float minWidth, float maxWidth, float minHeight, float maxHeight)
        {
            // Still Buggy!
            TopLeft.X = (TopLeft.X < minWidth ? minWidth : TopLeft.X);
            TopLeft.X = (TopLeft.X > maxWidth ? maxWidth : TopLeft.X);
            TopLeft.Y = (TopLeft.Y < minHeight ? 0 : TopLeft.Y);
            TopLeft.Y = (TopLeft.Y + ScreenBounds.Height > maxHeight ? maxHeight - ScreenBounds.Height : TopLeft.Y);
        }

        private static void FinalizeMatrix()
        {
            // Fixed on Top Left.
            Transform = Matrix.CreateTranslation(new Vector3(-TopLeft.X + StaticCamera.HorizontalLetterBox, -TopLeft.Y + StaticCamera.VerticalLetterBox, 0)) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 0));
        }
    }
}
