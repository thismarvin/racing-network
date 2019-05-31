using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.GameComponents;
using Racing.Engine.Entities.Geometry;

namespace Racing.Engine.Utilities
{
    static class StaticCamera
    {
        public static Matrix Transform { get; private set; }
        public static float Zoom { get; private set; }
        public static float VerticalLetterBox { get; private set; }
        public static float HorizontalLetterBox { get; private set; }

        static Shape topLetterBox;
        static Shape bottomLetterBox;
        static Shape leftLetterBox;
        static Shape rightLetterBox;

        public static void Initialize()
        {
            Reset(ScreenManager.DefaultWindowWidth, ScreenManager.DefaultWindowHeight);
        }

        private static void ResetZoom()
        {
            Zoom = Camera.Zoom;
        }

        private static void SetupLandscapeLetterBox(int windowWidth, int windowHeight)
        {
            VerticalLetterBox = (windowHeight / Zoom - Camera.ScreenBounds.Height * Camera.Scale) / 2;
            topLetterBox = new Shape(-128, -(int)VerticalLetterBox - 128, Camera.ScreenBounds.Width + 128 * 2, (int)VerticalLetterBox + 128, Color.Black);
            bottomLetterBox = new Shape(-128, Camera.ScreenBounds.Height, Camera.ScreenBounds.Width + 128 * 2, (int)VerticalLetterBox + 128, Color.Black);

            if (!ScreenManager.WideScreenSupport)
            {
                HorizontalLetterBox = (windowWidth / Zoom - Camera.ScreenBounds.Width * Camera.Scale) / 2;
                leftLetterBox = new Shape(-128 - (int)HorizontalLetterBox, -128, 128 + (int)HorizontalLetterBox, Camera.ScreenBounds.Height + 128 * 2, Color.Black);
                rightLetterBox = new Shape(Camera.ScreenBounds.Width, -128, (int)HorizontalLetterBox + 128, Camera.ScreenBounds.Height + 128 * 2, Color.Black);
            }
        }

        private static void SetupPortraitLetterBox(int windowWidth, int windowHeight)
        {
            HorizontalLetterBox = (windowWidth / Zoom - Camera.ScreenBounds.Width * Camera.Scale) / 2;
            leftLetterBox = new Shape(-(int)HorizontalLetterBox - 128, -128, (int)HorizontalLetterBox + 128, Camera.ScreenBounds.Height + 128 * 2, Color.Black);
            rightLetterBox = new Shape(Camera.ScreenBounds.Width, -128, (int)HorizontalLetterBox + 128, Camera.ScreenBounds.Height + 128 * 2, Color.Black);

            VerticalLetterBox = (windowHeight / Zoom - Camera.ScreenBounds.Height * Camera.Scale) / 2;
            topLetterBox = new Shape(-128, -(int)VerticalLetterBox - 128, Camera.ScreenBounds.Width + 128 * 2, (int)VerticalLetterBox + 128, Color.Black);
            bottomLetterBox = new Shape(-128, Camera.ScreenBounds.Height, Camera.ScreenBounds.Width + 128 * 2, (int)VerticalLetterBox + 128, Color.Black);
        }

        private static void FinalizeMatrix()
        {
            // Fixed on Top Left.
            Transform = Matrix.CreateTranslation(new Vector3(HorizontalLetterBox, VerticalLetterBox, 0)) *
                        Matrix.CreateScale(new Vector3(Zoom, Zoom, 0));
        }

        public static void Reset(int windowWidth, int windowHeight)
        {
            ResetZoom();
            switch (Game1.GameOrientation)
            {
                case Game1.Orientation.Landscape:
                    SetupLandscapeLetterBox(windowWidth, windowHeight);
                    break;
                case Game1.Orientation.Portrait:
                    SetupPortraitLetterBox(windowWidth, windowHeight);
                    break;
            }
            FinalizeMatrix();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Transform);
            {
                switch (Game1.GameOrientation)
                {
                    case Game1.Orientation.Landscape:
                        topLetterBox.Draw(spriteBatch);
                        bottomLetterBox.Draw(spriteBatch);
                        if (!ScreenManager.WideScreenSupport)
                        {
                            leftLetterBox.Draw(spriteBatch);
                            rightLetterBox.Draw(spriteBatch);
                        }
                        break;

                    case Game1.Orientation.Portrait:
                        leftLetterBox.Draw(spriteBatch);
                        rightLetterBox.Draw(spriteBatch);
                        break;
                }                
            }
            spriteBatch.End();
        }
    }
}