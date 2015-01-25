using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.Shared;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// Rendering utility.
    /// </summary>
    public class Renderer
    {
        /// <summary>
        /// Renders a black line box around a block.
        /// </summary>
        /// <param name="block">The block to highlight</param>
        public static void HighlightBlock(Location block)
        {
            GL.Color4(Color4.Black);
            Texture.White.Bind();
            Shader.ColorMultShader.Bind();
            GL.Begin(PrimitiveType.Lines);
            GL.Vertex3(block.X, block.Y, block.Z);
            GL.Vertex3(block.X + 1, block.Y, block.Z);
            GL.Vertex3(block.X + 1, block.Y, block.Z);
            GL.Vertex3(block.X + 1, block.Y + 1, block.Z);
            GL.Vertex3(block.X + 1, block.Y + 1, block.Z);
            GL.Vertex3(block.X, block.Y + 1, block.Z);
            GL.Vertex3(block.X, block.Y + 1, block.Z);
            GL.Vertex3(block.X, block.Y, block.Z);
            GL.Vertex3(block.X, block.Y, block.Z + 1);
            GL.Vertex3(block.X + 1, block.Y, block.Z + 1);
            GL.Vertex3(block.X + 1, block.Y, block.Z + 1);
            GL.Vertex3(block.X + 1, block.Y + 1, block.Z + 1);
            GL.Vertex3(block.X + 1, block.Y + 1, block.Z + 1);
            GL.Vertex3(block.X, block.Y + 1, block.Z + 1);
            GL.Vertex3(block.X, block.Y + 1, block.Z + 1);
            GL.Vertex3(block.X, block.Y, block.Z + 1);
            GL.Vertex3(block.X, block.Y, block.Z);
            GL.Vertex3(block.X, block.Y, block.Z + 1);
            GL.Vertex3(block.X + 1, block.Y, block.Z);
            GL.Vertex3(block.X + 1, block.Y, block.Z + 1);
            GL.Vertex3(block.X + 1, block.Y + 1, block.Z);
            GL.Vertex3(block.X + 1, block.Y + 1, block.Z + 1);
            GL.Vertex3(block.X, block.Y + 1, block.Z);
            GL.Vertex3(block.X, block.Y + 1, block.Z + 1);
            GL.End();
        }
    }
}
