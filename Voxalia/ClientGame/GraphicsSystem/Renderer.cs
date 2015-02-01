using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.Shared;
using Voxalia.ClientGame.WorldSystem;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// Rendering utility.
    /// </summary>
    public class Renderer
    {
        /// <summary>
        /// textures/common/item_frame
        /// </summary>
        public static Texture ItemFrame;

        /// <summary>
        /// Prepare the renderer.
        /// </summary>
        public static void Init()
        {
            ItemFrame = Texture.GetTexture("common/item_frame");
        }

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

        /// <summary>
        /// Renders a 2D rectangle.
        /// </summary>
        /// <param name="xmin">The lower bounds of the the rectangle: X coordinate</param>
        /// <param name="ymin">The lower bounds of the the rectangle: Y coordinate</param>
        /// <param name="xmax">The upper bounds of the the rectangle: X coordinate</param>
        /// <param name="ymax">The upper bounds of the the rectangle: Y coordinate</param>
        public static void RenderRectangle(int xmin, int ymin, int xmax, int ymax)
        {
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(1, 0);
            GL.Vertex3(xmax, ymin, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex3(xmax, ymax, 0);
            GL.TexCoord2(0, 1);
            GL.Vertex3(xmin, ymax, 0);
            GL.TexCoord2(0, 0);
            GL.Vertex3(xmin, ymin, 0);
            GL.End();
        }

        /// <summary>
        /// Renders an item on the 2D screen.
        /// </summary>
        /// <param name="item">The item to render</param>
        /// <param name="pos">Where to render it</param>
        /// <param name="size">How big to render it, in pixels</param>
        public static void RenderItem(Item item, Location pos, int size)
        {
            Shader.ColorMultShader.Bind();
            GL.Color4(Color4.White);
            ItemFrame.Bind();
            RenderRectangle((int)pos.X - 1, (int)pos.Y - 1, (int)(pos.X + size) + 1, (int)(pos.Y + size) + 1);
            item.Image.Bind();
            if (item.ImageModifier != null)
            {
                item.ImageModifier.Bind();
            }
            GL.Color4(item.Color);
            RenderRectangle((int)pos.X, (int)pos.Y, (int)(pos.X + size), (int)(pos.Y + size));
        }
    }
}
