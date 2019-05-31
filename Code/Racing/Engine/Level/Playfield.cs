using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Racing.Engine.Utilities;
using Racing.Engine.GameComponents;
using Racing.Engine.Entities;
using Racing.Engine.Entities.Geometry;
using Racing.Engine.Entities.World;
using Racing.Engine.AI.Evolution;
using Racing.Engine.Resources;

namespace Racing.Engine.Level
{
    static class Playfield
    {
        public static List<Entity> Entities { get; private set; }
        public static List<Entity> EntityBuffer { get; private set; }
        public static List<Player> Players { get; private set; }
        public static List<Shape> BoundingBoxes { get; private set; }

        public static Track RaceTrack { get; private set; }

        public static Vector2 CameraLocation { get; private set; }
        public static Random RNG { get; set; }
        public static bool Multiplayer { get; private set; }
        public static bool GameOver { get; set; }

        static Input input;
        private static bool noSpam;

        static int totalPlayers;

        static bool start;

        static List<Color> colors;

        static Text generationsText;
        static Number generationsNumber;

        static Text startPrompt;
        static Text resetPrompt;
        static Text editPrompt;
        static Text networkPrompt;
        static Text loadingPrompt;

        public static bool EditTrack { get; private set; }
        static bool showNetwork;
        static bool loading;

        static Timer loadAlgorithm;

        public static void Initialize()
        {
            Entities = new List<Entity>();
            EntityBuffer = new List<Entity>();
            Players = new List<Player>();
            BoundingBoxes = new List<Shape>();

            colors = new List<Color>();
            RNG = new Random(DateTime.Now.Millisecond);
            
            Reset();
        }

        public static void Reset()
        {
            GameOver = false;
            
            totalPlayers = 250;

            Entities.Clear();
            Players.Clear();
            
            input = new Input(PlayerIndex.One);

            RaceTrack = new Track(Camera.ScreenBounds.Width / 2, Camera.ScreenBounds.Height / 2, 20);
            Entities.Add(RaceTrack);

            generationsText = new Text(8, 8, "Generation", Sprite.Type.Text8x8);
            generationsNumber = new Number(8, 8 + 2 + 8, 1, 4, 9999, Sprite.Type.Text8x8);

            startPrompt = new Text(8, Camera.ScreenBounds.Height - 8 - 8 - 2 - 8 - 2 - 8 - 2 - 8, "Start", 1, Sprite.Type.Text8x8, new Color(Color.White, 255));
            resetPrompt = new Text(8, Camera.ScreenBounds.Height - 8 - 8 - 2 - 8 - 2 - 8, "Reset", 1, Sprite.Type.Text8x8, new Color(Color.White, 255));
            editPrompt = new Text(8, Camera.ScreenBounds.Height - 8 - 8 - 2 - 8, "Edit Track", 1, Sprite.Type.Text8x8, new Color(Color.White, 255));
            networkPrompt = new Text(8, Camera.ScreenBounds.Height - 8 - 8, "Draw Neural Network", 1, Sprite.Type.Text8x8, new Color(Color.White, 255));
            loadingPrompt = new Text(Camera.ScreenBounds.Width - 8 - 7 * 8, Camera.ScreenBounds.Height - 8 - 8, "Loading...", Sprite.Type.Text8x8);

            start = false;
            EditTrack = true;
            showNetwork = false;

            loading = false;
            loadAlgorithm = new Timer(500);

            HUD.Reset();
            NE.Reset();
        }

        private static void ResetPlayers()
        {
            foreach (Entity e in Entities)
            {
                if (e is Player)
                {
                    e.Remove = true;
                }
            }

            Players.Clear();

            for (int i = 0; i < totalPlayers; i++)
            {
                Players.Add(new Player(RaceTrack.Start.X, RaceTrack.Start.Y));
            }

            Players[0].ActivateInputControls();

            foreach (Player p in Players)
            {
                Entities.Add(p);
            }
        }

        public static void SetBoundingBoxes(List<Shape> boundingBoxes)
        {
            BoundingBoxes = boundingBoxes;
        }

        public static void AddEntity(Entity e)
        {
            EntityBuffer.Add(e);
        }

        private static void BackToMenu()
        {
            Reset();
            Game1.GameMode = Game1.Mode.Menu;
        }

        private static void CameraHandler(GameTime gameTime)
        {
            Camera.Update();
        }

        private static void UpdateInput(GameTime gameTime)
        {
            input.Update(gameTime);
            if (input.KeyState.IsKeyDown(Keys.R) && noSpam)
            {
                Reset();
                noSpam = false;
            }
            
            if (input.KeyState.IsKeyDown(Keys.Enter) && input.KeyReleased)
            {
                QueueTraining();
                input.KeyReleased = false;
            }

            if (input.KeyState.IsKeyDown(Keys.Tab) && input.KeyReleased)
            {
                if (NE.BestPlayer != null)
                    NE.BestPlayer.Brain.Save();
                input.KeyReleased = false;
            }

            if (input.DynamicCollisionRectangle.Intersects(startPrompt.Bounds.CollisionRectangle))
            {
                startPrompt.SetScale(1.1f);
                if (input.LeftClick() && noSpam)
                {
                    QueueTraining();
                    noSpam = false;
                }
            }
            else
            {
                startPrompt.SetScale(1);
            }

            if (input.DynamicCollisionRectangle.Intersects(resetPrompt.Bounds.CollisionRectangle))
            {
                resetPrompt.SetScale(1.1f);
                if (input.LeftClick() && noSpam)
                {
                    Reset();
                    noSpam = false;
                }
            }
            else
            {
                resetPrompt.SetScale(1);
            }

            if (input.DynamicCollisionRectangle.Intersects(editPrompt.Bounds.CollisionRectangle))
            {
                editPrompt.SetScale(1.1f);
                if (input.LeftClick() && noSpam)
                {
                    SetEditMode(!EditTrack);
                    noSpam = false;
                }
            }
            else
            {
                editPrompt.SetScale(1);
                if (!EditTrack)
                {
                    editPrompt.SetColor(new Color(Color.White, 150));
                }
                else
                {
                    editPrompt.SetColor(new Color(Color.White, 255));
                }
            }

            if (input.DynamicCollisionRectangle.Intersects(networkPrompt.Bounds.CollisionRectangle))
            {
                networkPrompt.SetScale(1.1f);
                if (input.LeftClick() && noSpam)
                {
                    showNetwork = !showNetwork;
                    noSpam = false;
                }
            }
            else
            {
                networkPrompt.SetScale(1);
                if (!showNetwork)
                {
                    networkPrompt.SetColor(new Color(Color.White, 150));
                }
                else
                {
                    networkPrompt.SetColor(new Color(Color.White, 255));
                }
            }

            noSpam = Keyboard.GetState().IsKeyUp(Keys.R) && !input.LeftClick() ? true : noSpam;
        }       

        private static void QueueTraining()
        {
            loading = true;
        }

        private static void StartTraining()
        {
            loading = false;
            start = true;
            ResetPlayers();
            SetEditMode(false);
        }

        private static void SetEditMode(bool on)
        {
            EditTrack = on;
            RaceTrack.SetEditMode(on);
        }
        
        private static void UpdateEntities(GameTime gameTime)
        {
            if (GameOver)
                return;

            for (int i = Entities.Count - 1; i >= 0; i--)
            {            
                Entities[i].Update(gameTime);

                if (Entities[i].Remove)
                    Entities.RemoveAt(i);
            }

            if (EntityBuffer.Count > 0)
            {
                foreach (Entity e in EntityBuffer)
                    Entities.Add(e);

                EntityBuffer.Clear();
            }

            //Entities.Sort();
        }

        public static void Update(GameTime gameTime)
        {
            CameraHandler(gameTime);
            UpdateInput(gameTime);

            UpdateEntities(gameTime);

            if (loading)
            {
                loadAlgorithm.Update(gameTime);
                if (loadAlgorithm.Done)
                {
                    StartTraining();
                    loadAlgorithm.Reset();
                }
            }

            if (start && NE.NextGeneration(Players))
            {
                foreach (Player p in Players)
                {
                    p.SetLocation(RaceTrack.Start.X, RaceTrack.Start.Y);
                }
                generationsNumber.Set(NE.Generations + 1);
            }

            NE.CalculateFitness(Players);

            if (NE.BestPlayer != null && showNetwork)
                NE.BestPlayer.Brain.VisualizeDecisions();
        }

        public static void Draw(SpriteBatch spriteBatch)
        {        
            foreach (Entity e in Entities)
            {
                e.Draw(spriteBatch);
            }

            if (NE.BestPlayer != null && showNetwork)
                NE.BestPlayer.Brain.Draw(spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Camera.Transform);
            {
                generationsText.Draw(spriteBatch);
                generationsNumber.Draw(spriteBatch);

                startPrompt.Draw(spriteBatch);
                resetPrompt.Draw(spriteBatch);
                editPrompt.Draw(spriteBatch);
                networkPrompt.Draw(spriteBatch);

                if (loading)
                    loadingPrompt.Draw(spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
