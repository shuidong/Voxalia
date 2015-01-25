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
using Voxalia.ClientGame.WorldSystem;
using Voxalia.ClientGame.EntitySystem;

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
                if (Window.Height > 0)
                {
                    Setup3D();
                    Run3D();
                    End3D();
                    Setup2D();
                    Run2D();
                    End2D();
                }
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Error / renderframe: " + ex.ToString());
            }
            // Final setup
            Window.SwapBuffers();
        }

        /// <summary>
        /// The camera eye location.
        /// </summary>
        public static Location CameraEye = new Location(0, 0, 1);

        /// <summary>
        /// The camera target location.
        /// </summary>
        public static Location CameraTarget = new Location(1, 0, 0);

        /// <summary>
        /// The camera up vector.
        /// </summary>
        public static Location CameraUp = new Location(0, 0, 1);

        /// <summary>
        /// The camera's field-of-view.
        /// </summary>
        public static float CameraFOV = 45;

        /// <summary>
        /// The camera's Z-Near factor.
        /// </summary>
        public static float CameraZNear = 0.1f;

        /// <summary>
        /// The camera's Z-Far factor.
        /// </summary>
        public static float CameraZFar = 10000f;

        static void Setup3D()
        {
            GL.Color4(Color4.White);
            Shader.ColorMultShader.Bind();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Enable(EnableCap.DepthTest);
            Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CameraFOV), Window.Width / Window.Height, CameraZNear, CameraZFar);
            Matrix4 view = Matrix4.LookAt(CameraEye.ToOVector(), CameraTarget.ToOVector(), CameraUp.ToOVector());
            GL.MultMatrix(ref proj);
            GL.MultMatrix(ref view);
        }

        static void Run3D()
        {
            foreach (KeyValuePair<Location, Chunk> chunkdata in Chunks)
            {
                chunkdata.Value.Render();
            }
            foreach (Entity ent in Entities)
            {
                ent.Render3D();
            }
            GL.Disable(EnableCap.DepthTest);
            if (!ThePlayer.SelectedBlock.IsNaN())
            {
                Location targ = ThePlayer.SelectedBlock.GetBlockLocation();
                Renderer.HighlightBlock(targ);
            }
        }

        static void End3D()
        {
        }

        static void Setup2D()
        {
            GL.Disable(EnableCap.DepthTest);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            Matrix4 ortho = Matrix4.CreateOrthographicOffCenter(0, Window.Width, Window.Height, 0, -1, 1);
            GL.MultMatrix(ref ortho);
        }

        static void Run2D()
        {
            FontSet.Standard.DrawColoredText("^0^e^7Hello World! " + ThePlayer.Position.ToString(), new Location(0, 0, 0));
            UIConsole.Draw();
        }

        static void End2D()
        {
        }
    }
}
