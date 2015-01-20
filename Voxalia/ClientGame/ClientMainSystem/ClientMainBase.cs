using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using System.Diagnostics;
using Voxalia.ClientGame.GraphicsSystem;
using Voxalia.Shared;
using OpenTK.Graphics.OpenGL;
using Voxalia.ClientGame.UISystem;
using Voxalia.ClientGame.CommandSystem;
using Frenetic.CommandSystem;

namespace Voxalia.ClientGame.ClientMainSystem
{
    /// <summary>
    /// Central client entry point.
    /// </summary>
    public partial class ClientMain
    {
        /// <summary>
        /// The client's OpenGL rendering window.
        /// </summary>
        public static GameWindow Window;

        /// <summary>
        /// Starts up the client.
        /// </summary>
        public static void Init()
        {
            SysConsole.Output(OutputType.INFO, "Loading client...");
            SysConsole.Output(OutputType.INFO, "Loading command engine...");
            ClientOutputter output = new ClientOutputter();
            ClientCommands.Init(output);
            ClientCVar.Init(output);
            SysConsole.Output(OutputType.INFO, "Loading window...");
            Window = new GameWindow(800, 600, GraphicsMode.Default, Program.GameName, GameWindowFlags.FixedWindow);
            Window.UpdateFrame += new EventHandler<FrameEventArgs>(Window_UpdateFrame);
            Window.RenderFrame += new EventHandler<FrameEventArgs>(Window_RenderFrame);
            Window.Closed += new EventHandler<EventArgs>(Window_Closed);
            Window.Load += new EventHandler<EventArgs>(Window_Load);
            Window.KeyPress += new EventHandler<KeyPressEventArgs>(KeyHandler.PrimaryGameWindow_KeyPress);
            Window.KeyUp += new EventHandler<KeyboardKeyEventArgs>(KeyHandler.PrimaryGameWindow_KeyUp);
            Window.KeyDown += new EventHandler<KeyboardKeyEventArgs>(KeyHandler.PrimaryGameWindow_KeyDown);
            Window.MouseDown += new EventHandler<MouseButtonEventArgs>(KeyHandler.Mouse_ButtonDown);
            Window.MouseUp += new EventHandler<MouseButtonEventArgs>(KeyHandler.Mouse_ButtonUp);
            SysConsole.Output(OutputType.INFO, "Running window startup sequence...");
            Window.Run(60, 60);
        }

        /// <summary>
        /// Called at first-time load, loads required resources.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The empty event args object</param>
        static void Window_Load(object sender, EventArgs e)
        {
            SysConsole.Output(OutputType.INFO, "Loading texture engine...");
            Texture.InitTextureSystem();
            SysConsole.Output(OutputType.INFO, "Loading shader engine...");
            Shader.InitShaderSystem();
            SysConsole.Output(OutputType.INFO, "Loading text engine...");
            GLFont.Init();
            SysConsole.Output(OutputType.INFO, "Loading font-set engine...");
            FontSet.Init();
            SysConsole.Output(OutputType.INFO, "Adjusting OpenGL settings...");
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.Blend);
            GL.Enable(EnableCap.Texture2D);
            GL.Viewport(0, 0, Window.Width, Window.Height);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            SysConsole.Output(OutputType.INFO, "Loading keyboard handling engine...");
            KeyHandler.Init();
            SysConsole.Output(OutputType.INFO, "Loading interactive console engine...");
            UIConsole.InitConsole();
            SysConsole.Output(OutputType.INFO, "Displaying window...");
        }

        /// <summary>
        /// Ends the program when the window is closed.
        /// Called by the window being closed.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The empty event args object</param>
        static void Window_Closed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
    }
}
