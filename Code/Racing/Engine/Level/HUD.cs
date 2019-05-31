using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Utilities;

namespace Racing.Engine.Level
{
    static class HUD
    {
        static Pinhole pinhole;

        static public void Initialize()
        {
            Reset();
        }

        public static void Reset()
        {
            pinhole = new Pinhole(Camera.ScreenBounds.Width / 2, Camera.ScreenBounds.Height / 2, Pinhole.Type.Open);
        }

        public static void Update(GameTime gameTime)
        {
            pinhole.Update(gameTime);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, StaticCamera.Transform);
            {
                //pinhole.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
