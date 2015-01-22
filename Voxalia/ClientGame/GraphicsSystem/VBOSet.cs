using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;

namespace Voxalia.ClientGame.GraphicsSystem
{
    public class VBOSet
    {
        List<RenderedVBO> VBOs = new List<RenderedVBO>();

        /// <summary>
        /// Get the RenderedVBO object for the given texture.
        /// </summary>
        /// <param name="text">The texture</param>
        /// <param name="color">The color to use</param>
        /// <returns>A non-null RenderedVBO object</returns>
        public RenderedVBO GetVBO(Texture text, Color4 color)
        {
            if (text == null)
            {
                return null;
            }
            int c = color.ToArgb();
            for (int i = 0; i < VBOs.Count; i++)
            {
                if (VBOs[i].VBOTexture.Internal_Texture == text.Internal_Texture
                    && VBOs[i].Color.ToArgb() == c)
                {
                    return VBOs[i];
                }
            }
            RenderedVBO vbo = new RenderedVBO();
            vbo.VBOTexture = text;
            vbo.Color = color;
            VBOs.Add(vbo);
            return vbo;
        }

        /// <summary>
        /// Builds all VBOs.
        /// </summary>
        public void Destroy()
        {
            for (int i = 0; i < VBOs.Count; i++)
            {
                VBOs[i].Destroy();
            }
            VBOs.Clear();
        }

        /// <summary>
        /// Builds all VBOs.
        /// </summary>
        public void Build()
        {
            for (int i = 0; i < VBOs.Count; i++)
            {
                VBOs[i].Build();
            }
        }

        /// <summary>
        /// Renders the whole VBO set.
        /// </summary>
        public void Render()
        {
            for (int i = 0; i < VBOs.Count; i++)
            {
                VBOs[i].Render();
            }
        }
    }
}
