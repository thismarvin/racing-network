﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Racing.Engine.Utilities
{
    class Input
    {
        PlayerIndex playerIndex;
        Timer timer;

        public KeyboardState KeyState { get; private set; }
        public bool KeyReleased { get; set; }

        public MouseState MouseState { get; private set; }
        public bool MouseReleased { get; set; }

        public enum InputType
        { AttackUp, AttackLeft, AttackDown, AttackRight, MovementUp, MovementLeft, MovementDown, MovementRight, Select, Back }

        public Vector2 DynamicCursorLocation { get; private set; }
        public Rectangle DynamicCollisionRectangle { get; private set; }

        public Vector2 StaticCursorLocation { get; private set; }
        public Rectangle StaticCollisionRectangle { get; private set; }

        public Input()
        {
            playerIndex = PlayerIndex.One;
            timer = new Timer(200);
        }

        public Input(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
            timer = new Timer(200);
        }

        public void Update(GameTime gameTime)
        {
            UpdateKeyboard(gameTime);
            UpdateMouse();
        }

        private void UpdateKeyboard(GameTime gameTime)
        {
            KeyState = Keyboard.GetState();
            if (!KeyReleased)
            {
                timer.Update(gameTime);
            }
            if (timer.Done)
            {
                KeyReleased = true;
                timer.Reset();
            }
        }

        private void UpdateMouse()
        {
            MouseState = Mouse.GetState();
            if (MouseState.LeftButton == ButtonState.Released)
            { MouseReleased = true; }

            DynamicCursorLocation = new Vector2(MouseState.X / Camera.Zoom / Camera.Scale + Camera.TopLeft.X / Camera.Scale - StaticCamera.HorizontalLetterBox / Camera.Scale, MouseState.Y / Camera.Zoom / Camera.Scale - StaticCamera.VerticalLetterBox + Camera.TopLeft.Y / Camera.Scale);
            DynamicCollisionRectangle = new Rectangle((int)DynamicCursorLocation.X, (int)DynamicCursorLocation.Y, 1, 1);
            StaticCursorLocation = new Vector2(MouseState.X / StaticCamera.Zoom / Camera.Scale - StaticCamera.HorizontalLetterBox / Camera.Scale, MouseState.Y / StaticCamera.Zoom / Camera.Scale - StaticCamera.VerticalLetterBox / Camera.Scale);
            StaticCollisionRectangle = new Rectangle((int)StaticCursorLocation.X, (int)StaticCursorLocation.Y, 1, 1);
        }

        public bool LeftClick()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
        }

        public bool RightClick()
        {
            return MouseState.RightButton == ButtonState.Pressed;
        }

        public bool IsKeyDown(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.AttackUp:
                    if (KeyState.IsKeyDown(Keys.Up)) { return true; }
                    break;
                case InputType.AttackLeft:
                    if (KeyState.IsKeyDown(Keys.Left)) { return true; }
                    break;
                case InputType.AttackDown:
                    if (KeyState.IsKeyDown(Keys.Down)) { return true; }
                    break;
                case InputType.AttackRight:
                    if (KeyState.IsKeyDown(Keys.Right)) { return true; }
                    break;
                case InputType.MovementUp:
                    if (KeyState.IsKeyDown(Keys.W)) { return true; }
                    break;
                case InputType.MovementLeft:
                    if (KeyState.IsKeyDown(Keys.A)) { return true; }
                    break;
                case InputType.MovementDown:
                    if (KeyState.IsKeyDown(Keys.S)) { return true; }
                    break;
                case InputType.MovementRight:
                    if (KeyState.IsKeyDown(Keys.D)) { return true; }
                    break;
                case InputType.Select:
                    if (KeyState.IsKeyDown(Keys.Space)) { return true; }
                    break;
                case InputType.Back:
                    if (KeyState.IsKeyDown(Keys.Escape)) { return true; }
                    break;
            }
            return false;
        }

        public bool IsButtonDown(InputType inputType)
        {
            switch (inputType)
            {
                case InputType.AttackUp:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.RightThumbstickUp) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.Y)) { return true; }
                    break;
                case InputType.AttackLeft:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.RightThumbstickLeft) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.X)) { return true; }
                    break;
                case InputType.AttackDown:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.RightThumbstickDown) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.A)) { return true; }
                    break;
                case InputType.AttackRight:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.RightThumbstickRight) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.B)) { return true; }
                    break;
                case InputType.MovementUp:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.DPadUp) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.LeftThumbstickUp)) { return true; }
                    break;
                case InputType.MovementLeft:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.DPadLeft) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.LeftThumbstickLeft)) { return true; }
                    break;
                case InputType.MovementDown:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.DPadDown) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.LeftThumbstickDown)) { return true; }
                    break;
                case InputType.MovementRight:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.DPadRight) || GamePad.GetState(playerIndex).IsButtonDown(Buttons.LeftThumbstickRight)) { return true; }
                    break;
                case InputType.Select:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.A)) { return true; }
                    break;
                case InputType.Back:
                    if (GamePad.GetState(playerIndex).IsButtonDown(Buttons.Start)) { return true; }
                    break;
            }
            return false;
        }

        public bool Pressing(InputType inputType)
        {
            if (IsKeyDown(inputType) || IsButtonDown(inputType)) { return true; }
            return false;
        }
    }
}
