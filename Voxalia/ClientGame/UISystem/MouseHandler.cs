using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.ClientMainSystem;
using OpenTK.Input;
using System.Drawing;

namespace Voxalia.ClientGame.UISystem
{
    /// <summary>
    /// Handles mouse input.
    /// </summary>
    class MouseHandler
    {
        /// <summary>
        /// How much the mouse has moved this tick.
        /// </summary>
        public static Location MouseDelta = new Location();

        /// <summary>
        /// The current mouse state for this tick.
        /// </summary>
        public static MouseState CurrentMouse;

        /// <summary>
        /// The mouse state during the previous tick.
        /// </summary>
        public static MouseState PreviousMouse;

        /// <summary>
        /// How much the mouse was scrolled this tick.
        /// </summary>
        public static int MouseScroll = 0;


        /// <summary>
        /// Moves the mouse back to the center position.
        /// </summary>
        public static void CenterMouse()
        {
            Point center = ClientMain.Window.PointToScreen(new Point(ClientMain.Window.Width / 2, ClientMain.Window.Height / 2));
            Mouse.SetPosition(center.X, center.Y);
        }

        public static float pwheelstate;
        public static float cwheelstate;

        public static int MouseX()
        {
            return ClientMain.Window.Mouse.X;
        }

        public static int MouseY()
        {
            return ClientMain.Window.Mouse.Y;
        }

        public static bool LeftMousePressed()
        {
            return CurrentMouse.LeftButton == ButtonState.Pressed && PreviousMouse.LeftButton != ButtonState.Pressed;
        }

        public static bool RightMousePressed()
        {
            return CurrentMouse.RightButton == ButtonState.Pressed && PreviousMouse.RightButton != ButtonState.Pressed;
        }

        public static bool MiddleMousePressed()
        {
            return CurrentMouse.MiddleButton == ButtonState.Pressed && PreviousMouse.MiddleButton != ButtonState.Pressed;
        }

        public static bool LeftMouseReleased()
        {
            return CurrentMouse.LeftButton == ButtonState.Released && PreviousMouse.LeftButton != ButtonState.Released;
        }

        public static bool RightMouseReleased()
        {
            return CurrentMouse.RightButton == ButtonState.Released && PreviousMouse.RightButton != ButtonState.Released;
        }

        public static bool MiddleMouseReleased()
        {
            return CurrentMouse.MiddleButton == ButtonState.Released && PreviousMouse.MiddleButton != ButtonState.Released;
        }

        /// <summary>
        /// Updates mouse movement.
        /// </summary>
        public static void Tick()
        {
            if (ClientMain.Window.Focused)
            {
                PreviousMouse = CurrentMouse;
                CurrentMouse = Mouse.GetState();
                pwheelstate = cwheelstate;
                cwheelstate = CurrentMouse.WheelPrecise;
                MouseScroll = (int)(cwheelstate - pwheelstate);
            }
            else
            {
                cwheelstate = Mouse.GetState().WheelPrecise;
                pwheelstate = cwheelstate;
            }
        }
    }
}

