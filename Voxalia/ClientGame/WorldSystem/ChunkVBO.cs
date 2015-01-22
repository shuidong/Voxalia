using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.GraphicsSystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Voxalia.ClientGame.WorldSystem
{
    /// <summary>
    /// Holds a Vertex Buffer Object for a single texture within a chunk.
    /// </summary>
    public class ChunkVBO
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
        /// The texture this ChunkVBO uses.
        /// </summary>
        public Texture VBOTexture;

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
        /// Adds a flat side to the renderer.
        /// </summary>
        /// <param name="x">The absolute X coordinate</param>
        /// <param name="y">The absolute Y coordinate</param>
        /// <param name="z">The absolute Z coordinate</param>
        /// <param name="Normal">The normal of the side to add, must be a unit vector facing directly down an axis</param>
        public void AddSide(float x, float y, float z, Vector3 Normal)
        {
            // TODO: Simplify
            if (Normal.Z == -1)
            {
                Texs.Add(new Vector2(0, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(0, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y + 1, z));
                Inds.Add((uint)(Vecs.Count - 1));
            }
            else if (Normal.Z == 1)
            {
                Texs.Add(new Vector2(0, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(0, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y + 1, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));
            }
            else if (Normal.X == -1)
            {
                Texs.Add(new Vector2(0, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y + 1, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y + 1, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(0, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));
            }
            else if (Normal.X == 1)
            {
                Texs.Add(new Vector2(0, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(0, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z));
                Inds.Add((uint)(Vecs.Count - 1));
            }
            else if (Normal.Y == -1)
            {
                Texs.Add(new Vector2(0, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(0, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y, z));
                Inds.Add((uint)(Vecs.Count - 1));
            }
            else if (Normal.Y == 1)
            {
                Texs.Add(new Vector2(0, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y + 1, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 1));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(1, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));

                Texs.Add(new Vector2(0, 0));
                Norms.Add(new Vector3(0, 0, 1));
                Vecs.Add(new Vector3(x, y + 1, z + 1));
                Inds.Add((uint)(Vecs.Count - 1));
            }
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
            //GL.Finish();
            GL.PopClientAttrib();
        }
    }
}
