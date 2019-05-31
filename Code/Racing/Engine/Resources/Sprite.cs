using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.GameComponents;
using Racing.Engine.Utilities;

namespace Racing.Engine.Resources
{
    class Sprite : MonoObject
    {
        Texture2D spriteSheet;
        Rectangle sourceRectangle;
        Rectangle locationRectangle;
        public Color ObjectColor { get; private set; }
        public Vector2 Center { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public float Rotation { get; set; }
        public bool Show { get; set; }        
        public SpriteEffects Effect { get; set; }
        public Type CurrentSprite { get; private set; }
        int originalFrameX;
        int originalFrameY;
        int frameX;
        int frameY;
        Vector2 scale;

        public int LayerDepth { get; set; }
        public int Frame { get; private set; }

        public enum Type
        {
            Text8x8, Colon8,
            Text16x16, Colon16,
            Text19x19,

            Car, Roof,

            None
        }

        public Sprite(float x, float y, Type sprite) : base(x, y)
        {
            spriteSheet = Assets.Sprites;
            ObjectColor = Color.White;
            Center = Vector2.Zero;
            Rotation = 0;
            Show = true;
            CurrentSprite = sprite;
            scale = new Vector2(Camera.Scale, Camera.Scale);
            InitializeSprite();
        }

        public Sprite(float x, float y, int frame, int columns, Type sprite) : this(x, y, sprite)
        {
            SetFrame(frame, columns);
        }

        private void InitializeSprite()
        {
            switch (CurrentSprite)
            {
                #region Text
                case Type.Text8x8:
                    SpriteSetup(0, 0, 8, 8);
                    spriteSheet = Assets.Text8x8;
                    break;
                case Type.Colon8:
                    SpriteSetup(0, 0, 8, 8);
                    SetFrame(":".ToCharArray()[0], 16);
                    spriteSheet = Assets.Text8x8;
                    break;
                case Type.Text16x16:
                    SpriteSetup(0, 0, 16, 16);
                    spriteSheet = Assets.Text16x16;
                    break;
                case Type.Colon16:
                    SpriteSetup(0, 0, 16, 16);
                    SetFrame(":".ToCharArray()[0], 16);
                    spriteSheet = Assets.Text16x16;
                    break;
                case Type.Text19x19:
                    SpriteSetup(0, 0, 19, 19);
                    spriteSheet = Assets.Text19x19;
                    break;
                #endregion

                case Type.Car:
                    SpriteSetup(0, 0, 16, 16);
                    spriteSheet = Assets.Player;
                    break;
                case Type.Roof:
                    SpriteSetup(0, 16, 16, 16);
                    spriteSheet = Assets.Player;
                    break;

                case Type.None:
                    SpriteSetup(0, 0, 0, 0);
                    break;
            }
            sourceRectangle = new Rectangle(frameX, frameY, Width, Height);
        }

        private void SpriteSetup(int frameX, int frameY, int width, int height)
        {
            this.frameX = frameX;
            this.frameY = frameY;
            originalFrameX = frameX;
            originalFrameY = frameY;
            Width = width;
            Height = height;
        }

        public void IncrementFrameX(int pixels)
        {
            frameX += pixels;
            sourceRectangle = new Rectangle(frameX, frameY, Width, Height);
        }

        public void IncrementFrameY(int pixels)
        {
            frameY += pixels;
            sourceRectangle = new Rectangle(frameX, frameY, Width, Height);
        }

        public void SetFrame(int frame, int columns)
        {
            frameX = originalFrameX + frame % columns * Width;
            frameY = originalFrameY + frame / columns * Height;
            sourceRectangle = new Rectangle(frameX, frameY, Width, Height);
        }

        public void SetSpriteSheet(Texture2D texture)
        {
            spriteSheet = texture;
        }

        public void SetSprite(Type newSprite)
        {
            CurrentSprite = newSprite;
            InitializeSprite();
        }

        public void SetColor(Color color)
        {
            ObjectColor = color;
        }

        public void SetCenter(float xOffset, float yOffset)
        {
            Center = new Vector2(xOffset, yOffset);
        }

        public void SetScale(float scale)
        {
            this.scale = new Vector2(scale * Camera.Scale, scale * Camera.Scale);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            locationRectangle = new Rectangle((int)Location.X, (int)Location.Y, Width, Height);

            if (Show)
            {
                spriteBatch.Draw(spriteSheet, ScaledLocation, sourceRectangle, ObjectColor, Rotation, Center, scale, Effect, Y);
            }
        }
    }
}
