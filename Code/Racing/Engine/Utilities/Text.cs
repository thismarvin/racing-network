using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Racing.Engine.Resources;
using Racing.Engine.GameComponents;
using Racing.Engine.Entities.Geometry;

namespace Racing.Engine.Utilities
{
    class Text : MonoObject
    {
        public string Dialogue { get; private set; }

        List<string> words;
        List<Sprite> sprites;
        Timer timer;
        Sprite.Type textType;
        float textWidth;
        int maximumCharacterCount;
        float spacing;       
        bool showAll;
        float scale;    
        bool compact;

        public Shape Bounds { get; private set; }
        Color color;

        public Text(float x, float y, string dialogue, float scale, Sprite.Type type, Color color) : base(x, y)
        {
            words = new List<string>();
            sprites = new List<Sprite>();
            timer = new Timer(0);

            Dialogue = dialogue;
            textType = type;
            maximumCharacterCount = dialogue.Length * 2;
            compact = true;
            showAll = true;
            this.scale = scale;

            BreakUpWords();
            CreateText();
            SetColor(color);
        }

        public Text(float x, float y, string dialogue, Sprite.Type type) : this(x, y, dialogue, 1, type, Color.White)
        {

        }

        private void TextSetup()
        {
            switch (textType)
            {
                case Sprite.Type.Text8x8:
                    textWidth = 7;
                    spacing = 2;
                    break;
                case Sprite.Type.Text16x16:
                    textWidth = 14;
                    spacing = 4;
                    break;
                case Sprite.Type.Text19x19:
                    Dialogue = Dialogue.ToUpper();
                    textWidth = 14;
                    spacing = 8;
                    break;
            }

            textWidth *= scale;
            spacing *= scale;
        }

        public void SetCompact(bool compact)
        {
            this.compact = compact;
            CreateText();
        }

        public void SetScale(float scale)
        {
            this.scale = scale;
            CreateText();
        }

        public void SetText(string text)
        {
            Dialogue = text;
            BreakUpWords();
            CreateText();
        }

        public void SetColor(Color color)
        {
            this.color = color;
            foreach(Sprite s in sprites)
            {
                s.SetColor(this.color);
            }
        }

        private void BreakUpWords()
        {
            words.Clear();
            string[] wordsArray = Regex.Split(Dialogue, "[ ]+");
            foreach (string s in wordsArray)
            {
                words.Add(s);
            }
        }

        private void CreateText()
        {
            sprites.Clear();
            TextSetup();

            int dialougeIndex = 0;
            int lineIndex = 0;
            float wordLength = 0;
            int y = 0;

            foreach (string s in words)
            {
                if (s.Length + lineIndex + 1 > maximumCharacterCount)
                {
                    wordLength = 0;
                    lineIndex = 1;
                    y++;
                }

                for (int i = 0; i < s.Length; i++)
                {
                    sprites.Add(new Sprite((int)Location.X + wordLength, (int)Location.Y + ((textWidth + spacing) * y), textType));
                    sprites.Last().SetFrame(Dialogue.Substring(dialougeIndex, 1).ToCharArray()[0], 16);
                    sprites.Last().SetScale(scale);
                    if (!showAll) { sprites.Last().Show = false; }

                    if (compact)
                    {
                        if (s[i] == 'I' || s[i] == 'i' || s[i] == '!' || s[i] == 'l' || s[i] == '.' || s[i] == ',' || s[i] == '\'' || s[i] == ':' || s[i] == ';')
                        {
                            switch (textType)
                            {
                                case Sprite.Type.Text8x8:
                                    wordLength += 3 * scale;
                                    break;
                                case Sprite.Type.Text16x16:
                                    wordLength += 6 * scale;
                                    break;
                                case Sprite.Type.Text19x19:
                                    wordLength += 6 * scale;
                                    break;
                            }
                        }
                        else if (s[i] == 't')
                        {
                            switch (textType)
                            {
                                case Sprite.Type.Text8x8:
                                    wordLength += 5 * scale;
                                    break;
                                case Sprite.Type.Text16x16:
                                    wordLength += 10 * scale;
                                    break;
                                case Sprite.Type.Text19x19:
                                    wordLength += 10 * scale;
                                    break;
                            }
                        }
                        else if (s[i] == 'f')
                        {
                            switch (textType)
                            {
                                case Sprite.Type.Text8x8:
                                    wordLength += 6 * scale;
                                    break;
                                case Sprite.Type.Text16x16:
                                    wordLength += 12 * scale;
                                    break;
                                case Sprite.Type.Text19x19:
                                    wordLength += 12 * scale;
                                    break;
                            }
                        }
                        else
                        {
                            wordLength += textWidth;
                        }
                    }
                    else
                    {
                        if (s[i] == 'I')
                        {
                            sprites.Last().SetLocation(sprites.Last().X + 4 * scale, sprites.Last().Y);
                        }
                        wordLength += textWidth;
                    }

                    dialougeIndex++;
                    lineIndex++;
                }

                // Acounts for space between words.
                dialougeIndex++;
                lineIndex++;
                wordLength += textWidth;
            }

            Bounds = new Shape(X, Y, (int)Math.Ceiling((y == 0 ? wordLength - textWidth : maximumCharacterCount * textWidth)), (int)Math.Ceiling(textWidth * (y + 1)), 2, Palette.GrassGreen);

            sprites.Reverse();
        }

        public void Update(GameTime gameTimer)
        {
            timer.Update(gameTimer);

            if (timer.Done && !showAll)
            {
                for (int i = 0; i < sprites.Count; i++)
                {
                    if (!sprites[i].Show)
                    {
                        sprites[i].Show = true;
                        break;
                    }
                    if (i == sprites.Count - 1)
                    {
                        showAll = true;
                    }
                }
                timer.Reset();
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Sprite S in sprites)
            {
                S.Draw(spriteBatch);
            }

            if (Game1.DebugMode)
                Bounds.Draw(spriteBatch);
        }
    }
}
