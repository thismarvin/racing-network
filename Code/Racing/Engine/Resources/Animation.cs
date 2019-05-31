using System;
using Microsoft.Xna.Framework;
using Racing.Engine.Utilities;

namespace Racing.Engine.Resources
{
    class Animation
    {
        Sprite.Type[] frames;
        Timer timer;
        Type type;
        public int CurrentFrame { get; private set; }
        public int TotalFrames { get; private set; }
        public int Columns { get; private set; }
        public bool Finished { get; private set; }

        public enum Type
        {
            Loop, NoLoop
        }

        public Animation(Sprite.Type[] frames, float frameDuration)
        {
            this.frames = frames;
            TotalFrames = frames.Length;
            timer = new Timer(frameDuration);
            type = Type.Loop;
        }

        public Animation(Sprite.Type[] frames, float frameDuration, int currentFrame) : this(frames, frameDuration)
        {
            CurrentFrame = currentFrame;
        }

        public Animation(int totalFrames, float frameDuration)
        {
            TotalFrames = totalFrames;
            Columns = totalFrames;
            timer = new Timer(frameDuration);
            type = Type.Loop;
        }

        public Animation(int totalFrames, int columns, float frameDuration) : this(totalFrames, frameDuration)
        {
            Columns = columns;
        }

        public void Update(GameTime gameTime)
        {
            timer.Update(gameTime);
            if (timer.Done)
            {
                switch (type)
                {
                    case Type.Loop:
                        CurrentFrame = CurrentFrame >= TotalFrames - 1 ? 0 : ++CurrentFrame;
                        break;
                    case Type.NoLoop:
                        CurrentFrame = CurrentFrame >= TotalFrames - 1 ? TotalFrames : ++CurrentFrame;
                        break;
                }
                timer.Reset();
            }
            Finished = type == Type.NoLoop && CurrentFrame == TotalFrames;
        }

        public Sprite.Type CurrentSprite()
        {
            return frames[CurrentFrame];
        }

        public void SetCurrentFrame(int frame)
        {
            CurrentFrame = frame;
            timer.Reset();
        }

        public void SetType(Type type)
        {
            this.type = type;
        }
    }
}
