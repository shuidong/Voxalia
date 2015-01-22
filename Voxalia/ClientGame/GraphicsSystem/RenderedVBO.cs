using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Voxalia.ClientGame.GraphicsSystem
{
    public class RenderedVBO
    {
        uint VBO;
        uint VBONormals;
        uint VBOTexCoords;
        uint VBOIndices;
        Vector3[] Positions;
        Vector3[] Normals;
        Vector2[] TexCoords;
        uint[] Indices;
        List<Vector3> Vecs = new List<Vector3>();
        List<Vector3> Norms = new List<Vector3>();
        List<Vector2> Texs = new List<Vector2>();
        List<uint> Inds = new List<uint>();

        /// <summary>
        /// The texture this RenderedVBO uses.
        /// </summary>
        public Texture VBOTexture;

        /// <summary>
        /// The color this RenderedVBO uses.
        /// </summary>
        public Color4 Color;
        
        /// <summary>
        /// Destroys the internal VBO, so this can be safely deleted.
        /// </summary>
        public void Destroy()
        {
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(VBONormals);
            GL.DeleteBuffer(VBOTexCoords);
            GL.DeleteBuffer(VBOIndices);
        }

        /// <summary>
        /// Adds a 2D quad facing straight up.
        /// </summary>
        /// <param name="min">The vector minimum</param>
        /// <param name="max">The vector maximum</param>
        /// <param name="tmin">The texture minimum</param>
        /// <param name="tmax">The texture maximum</param>
        public void AddQuad(Vector3 min, Vector3 max, Vector2 tmin, Vector2 tmax)
        {
            Texs.Add(new Vector2(tmin.X, tmin.Y));
            Vecs.Add(new Vector3(min.X, min.Y, min.Z));
            Texs.Add(new Vector2(tmax.X, tmin.Y));
            Vecs.Add(new Vector3(max.X, min.Y, min.Z));
            Texs.Add(new Vector2(tmax.X, tmax.Y));
            Vecs.Add(new Vector3(max.X, max.Y, min.Z));
            Texs.Add(new Vector2(tmin.X, tmax.Y));
            Vecs.Add(new Vector3(min.X, max.Y, min.Z));
        }

        /// <summary>
        /// Turns the local VBO build information into an actual internal GPU-side VBO.
        /// </summary>
        public void Build()
        {
            Positions = Vecs.ToArray();
            Normals = Norms.ToArray();
            TexCoords = Texs.ToArray();
            Indices = Inds.ToArray();
            Vecs = new List<Vector3>();
            Norms = new List<Vector3>();
            Texs = new List<Vector2>();
            Inds = new List<uint>();
            // Normal buffer
            GL.GenBuffers(1, out VBONormals);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBONormals);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Normals.Length * Vector3.SizeInBytes),
            Normals, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // TexCoord buffer
            GL.GenBuffers(1, out VBOTexCoords);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTexCoords);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(TexCoords.Length * Vector2.SizeInBytes),
            TexCoords, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Vertex buffer
            GL.GenBuffers(1, out VBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Positions.Length * Vector3.SizeInBytes),
            Positions, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Index buffer
            GL.GenBuffers(1, out VBOIndices);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIndices);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(Indices.Length * sizeof(uint)),
            Indices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        /// <summary>
        /// Renders the internal VBO to screen.
        /// </summary>
        public void Render()
        {
            VBOTexture.Bind();
            GL.Color4(Color); // TODO: Set color through shader magic, not one color per VBO!
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBONormals);
            GL.NormalPointer(NormalPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBOTexCoords);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, Vector2.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.VertexPointer(3, VertexPointerType.Float, Vector3.SizeInBytes, IntPtr.Zero);
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, VBOIndices);
            GL.DrawElements(PrimitiveType.Quads, Indices.Length, DrawElementsType.UnsignedInt, IntPtr.Zero);
            GL.PopClientAttrib();
        }
    }
}
