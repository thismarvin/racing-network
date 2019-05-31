using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Racing.Engine.Resources
{
    static class Assets
    {
        public static Texture2D Player { get; private set; }
        public static Texture2D Sprites { get; private set; }
        public static Texture2D Text8x8 { get; private set; }
        public static Texture2D Text16x16 { get; private set; }
        public static Texture2D Text19x19 { get; private set; }

        public static void LoadContent(ContentManager Content)
        {
            Player = Content.Load<Texture2D>("Assets/Sprites/Player");
            Sprites = Content.Load<Texture2D>("Assets/Sprites/Sprites");
            Text8x8 = Content.Load<Texture2D>("Assets/Sprites/ASCII 8");
            Text16x16 = Content.Load<Texture2D>("Assets/Sprites/ASCII 16");
            Text19x19 = Content.Load<Texture2D>("Assets/Sprites/ASCII 19");
        }
    }
}
