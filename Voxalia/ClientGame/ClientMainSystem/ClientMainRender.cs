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
        static int gFPS = 0;
        static int gFPSc = 0;
        static double gFPSCounter = 0;

        /// <summary>
        /// The primary render entry point from the OpenGL window.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The frame event details, including delta timing</param>
        static void Window_RenderFrame(object sender, FrameEventArgs e)
        {
            // Initial Setup
            gFPSCounter += e.Time;
            if (gFPSCounter >= 1)
            {
                gFPS = gFPSc;
                gFPSc = 0;
                gFPSCounter = 0;
            }
            gFPSc++;
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

        static Matrix4 proj;
        static Matrix4 view;
        static Matrix4 combined;

        static void Setup3D()
        {
            GL.Color4(Color4.White);
            Shader.ThreeD_Geometry.Bind();
            GL.MatrixMode(MatrixMode.Projection);
            GL.Enable(EnableCap.DepthTest);
            proj = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(CameraFOV), Window.Width / Window.Height, CameraZNear, CameraZFar);
            view = Matrix4.LookAt(CameraEye.ToOVector(), CameraTarget.ToOVector(), CameraUp.ToOVector());
            combined = view * proj;
            //GL.MultMatrix(ref proj);
            //GL.MultMatrix(ref view);
            GL.MultMatrix(ref combined);
        }

        static void Run3D()
        {
            //Frustum frustum = new Frustum(combined);
            foreach (KeyValuePair<Location, Chunk> chunkdata in Chunks)
            {
                //Location min = new Location(chunkdata.Value.X * 30, chunkdata.Value.Y * 30, chunkdata.Value.Z * 30);
                //if (frustum.ContainsBox(min, min + new Location(30, 30, 30)))
                {
                    chunkdata.Value.Render();
                }
            }
            foreach (Entity ent in Entities)
            {
                ent.Render3D();
            }
        }

        static void End3D()
        {
            GL.Disable(EnableCap.DepthTest);
            if (!ThePlayer.SelectedBlock.IsNaN())
            {
                Location targ = ThePlayer.SelectedBlock.GetBlockLocation();
                Renderer.HighlightBlock(targ);
            }
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
            // Draw some debug output
            FontSet.Standard.DrawColoredText("^0(Debug)Position: " + ThePlayer.Position.ToString() + "\n" + gFPS, new Location(0, 0, 0));
            // Draw the quickbar
            int center = Window.Width / 2;
            Renderer.RenderItem(GetItemForSlot(QuickBarPos - 2), new Location(center - (32 + 32 + 32 + 3), Window.Height - (32 + 16), 0), 32);
            Renderer.RenderItem(GetItemForSlot(QuickBarPos - 1), new Location(center - (32 + 32 + 2), Window.Height - (32 + 16), 0), 32);
            Renderer.RenderItem(GetItemForSlot(QuickBarPos + 1), new Location(center + (32 + 1), Window.Height - (32 + 16), 0), 32);
            Renderer.RenderItem(GetItemForSlot(QuickBarPos + 2), new Location(center + (32 + 32 + 2), Window.Height - (32 + 16), 0), 32);
            Renderer.RenderItem(GetItemForSlot(QuickBarPos), new Location(center - (32 + 1), Window.Height - 64, 0), 64);
            // Draw the console
            UIConsole.Draw();
        }

        static void End2D()
        {
        }
    }
}
