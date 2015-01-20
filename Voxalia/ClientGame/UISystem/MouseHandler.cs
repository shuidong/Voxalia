using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.ClientGame.CommandSystem;
using OpenTK.Input;
using System.Drawing;

namespace Voxalia.ClientGame.UISystem
{
    /// <summary>
    /// Handles mouse input.
    /// </summary>
    public class MouseHandler
    {
        /// <summary>
        /// Whether the mouse is captured.
        /// </summary>
        public static bool MouseCaptured = false;

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
        /// Captures the mouse to this window.
        /// </summary>
        public static void CaptureMouse()
        {
            if (UIConsole.Open)
            {
                UIConsole.MouseWasCaptured = true;
                return;
            }
            CenterMouse();
            MouseCaptured = true;
            ClientMain.Window.CursorVisible = false;
        }

        /// <summary>
        /// Uncaptures the mouse for this window.
        /// </summary>
        public static void ReleaseMouse()
        {
            MouseCaptured = false;
            ClientMain.Window.CursorVisible = true;
        }

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

        /// <summary>
        /// Updates mouse movement.
        /// </summary>
        public static void Tick()
        {
            if (ClientMain.Window.Focused && MouseCaptured && !UIConsole.Open)
            {
                double MoveX = (((ClientMain.Window.Width / 2) - MouseX()) * ClientCVar.u_mouse_sensitivity.ValueD);
                double MoveY = (((ClientMain.Window.Height / 2) - MouseY()) * ClientCVar.u_mouse_sensitivity.ValueD);
                MouseDelta = new Location((float)MoveX, (float)MoveY, 0);
                CenterMouse();
                PreviousMouse = CurrentMouse;
                CurrentMouse = Mouse.GetState();
                pwheelstate = cwheelstate;
                cwheelstate = CurrentMouse.WheelPrecise;
                MouseScroll = (int)(cwheelstate - pwheelstate);
            }
            else
            {
                MouseDelta = Location.Zero;
            }
            if (ClientMain.Window.Focused && !MouseCaptured)
            {
                PreviousMouse = CurrentMouse;
                CurrentMouse = Mouse.GetState();
                pwheelstate = cwheelstate;
                cwheelstate = CurrentMouse.WheelPrecise;
                MouseScroll = (int)(cwheelstate - pwheelstate);
            }
            if (!ClientMain.Window.Focused)
            {
                cwheelstate = Mouse.GetState().WheelPrecise;
                pwheelstate = cwheelstate;
            }
        }
    }
}
