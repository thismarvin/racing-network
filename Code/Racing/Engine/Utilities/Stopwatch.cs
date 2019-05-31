using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Resources;
using Racing.Engine.GameComponents;

namespace Racing.Engine.Utilities
{
    class Stopwatch : MonoObject
    {
        List<Number> numbers;
        List<Sprite> sprites;
        Timer timer;
        int increment;
        public bool Done { get; private set; }
        Type type;
        public enum Type
        {
            CountUp, CountDown
        }

        public Stopwatch(float x, float y, string currentTime, Type timerType, Sprite.Type spriteType) : base(x, y)
        {
            numbers = new List<Number>();
            sprites = new List<Sprite>();
            type = timerType;
            int textWidth = 0;
            increment = type == Type.CountUp ? 1 : -1;

            switch (spriteType)
            {
                case Sprite.Type.Text8x8:
                    textWidth = 8;
                    break;
            }

            timer = new Timer(1000);

            int colonCount = 0;
            for (int i = 0; i < currentTime.Length; i++)
            {
                colonCount = currentTime.Substring(i, 1) == ":" ? ++colonCount : colonCount;
            }

            int offset = (int)X;
            int index = 0;

            for (int i = 0; i < colonCount + 1; i++)
            {
                numbers.Add(new Number(offset, y, ParseTime(index, currentTime), 2, 60, spriteType));
                offset += textWidth * 2;
                if (i != colonCount)
                {
                    sprites.Add(new Sprite(offset, y, Sprite.Type.Colon8));
                }
                offset += 4;
                index += 3;
            }
        }

        private int ParseTime(int index, string currentTime)
        {
            return int.Parse(currentTime.Substring(index, 2));
        }

        public void Update(GameTime gameTime)
        {
            TimerLogic(gameTime);
        }

        private void TimerLogic(GameTime gameTime)
        {
            timer.Update(gameTime);

            if (timer.Done && !Done)
            {
                numbers[numbers.Count - 1].Increment(increment);
                timer.Reset();
            }

            if (numbers[numbers.Count - 1].CurrentValue >= 60 || numbers[numbers.Count - 1].CurrentValue < 0)
            {
                if (type == Type.CountUp)
                {
                    numbers[numbers.Count - 1].Set(0);
                }
                else
                {
                    numbers[numbers.Count - 1].Set(59);
                }

                numbers[numbers.Count - 2].Increment(increment);
            }

            if (numbers[0].CurrentValue == 60 || (numbers[0].CurrentValue == 0 && numbers[1].CurrentValue == 0))
            {
                Done = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Number N in numbers)
            {
                N.Draw(spriteBatch);
            }

            foreach (Sprite S in sprites)
            {
                S.Draw(spriteBatch);
            }
        }
    }
}
