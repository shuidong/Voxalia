using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.ClientGame.GraphicsSystem;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.ClientMainSystem
{
    public partial class ClientMain
    {
        /// <summary>
        /// The primary render entry point from the OpenGL window.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The frame event details, including delta timing</param>
        static void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            // Initial Setup
            GL.ClearColor(Color4.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            // General rendering
            try
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.PushMatrix();
                // TODO: 3D
                GL.Enable(EnableCap.DepthTest);
                GL.PopMatrix();
                GL.PushMatrix();
                // 2D
                Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Window.Width, Window.Height, 0, -1, 1);
                GL.MultMatrix(ref ortho);
                GL.Disable(EnableCap.DepthTest);
                FontSet.Standard.DrawColoredText("^0^e^7Hello World!", new Location(0, 0, 0));
                UIConsole.Draw();
                GL.PopMatrix();
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Error / renderframe: " + ex.ToString());
            }
            // Final setup
            Window.SwapBuffers();
        }
    }
}
