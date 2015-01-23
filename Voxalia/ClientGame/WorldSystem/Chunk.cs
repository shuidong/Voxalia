using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.EntitySystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.ClientGame.GraphicsSystem;
using Voxalia.ClientGame.ClientMainSystem;
using BulletSharp;

namespace Voxalia.ClientGame.WorldSystem
{
    /// <summary>
    /// Represents a part of the world, 30 wide, 30 long, 30 tall.
    /// </summary>
    public class Chunk
    {
        /// <summary>
        /// The X coordinate of the chunk. X * 30 = actual coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// The Y coordinate of the chunk. Y * 30 = actual coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// The Z coordinate of the chunk. Z * 30 = actual coordinate.
        /// </summary>
        public int Z;

        /// <summary>
        /// The physics world static body of this chunk.
        /// </summary>
        public RigidBody Body = null;

        /// <summary>
        /// All blocks within the chunk.
        /// </summary>
        public InternalBlockData[, ,] Blocks;

        /// <summary>
        /// All entities loaded in this chunk.
        /// </summary>
        public List<Entity> Entities;

        /// <summary>
        /// All entities loaded in this chunk that should tick.
        /// </summary>
        public List<Entity> Tickers;

        /// <summary>
        /// Constructs a chunk, made up of air by default.
        /// </summary>
        /// <param name="x">The x coordinate</param>
        /// <param name="y">The y coordinate</param>
        /// <param name="z">The z coordinate</param>
        public Chunk(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
            Blocks = new InternalBlockData[30, 30, 30];
            Entities = new List<Entity>();
            Tickers = new List<Entity>();
        }

        /// <summary>
        /// Sets a block within the chunk to the specified type.
        /// </summary>
        /// <param name="localX">The X coordinate, relative to the chunk</param>
        /// <param name="localY">The Y coordinate, relative to the chunk</param>
        /// <param name="localZ">The Z coordinate, relative to the chunk</param>
        /// <param name="type">The type of the block</param>
        public void SetBlock(int localX, int localY, int localZ, ushort type)
        {
            Blocks[localX, localY, localZ].Type = type;
        }

        /// <summary>
        /// Gets a block object representing the block at a given coordinate relative to the chunk.
        /// </summary>
        /// <param name="relX">The X coordinate, relative to the chunk</param>
        /// <param name="relY">The Y coordinate, relative to the chunk</param>
        /// <param name="relZ">The Z coordinate, relative to the chunk</param>
        /// <returns>A representative Block object</returns>
        public Block GetBlock(int localX, int localY, int localZ)
        {
            return new Block(this, localX, localY, localZ);
        }

        public void FromBytes(byte[] data)
        {
            SysConsole.Output(OutputType.INFO, "Prepare chunk at " + X + ", " + Y + ", " + Z);
            if (data.Length == 30 * 30 * 30 * 2)
            {
                int index = 0;
                for (int x = 0; x < 30; x++)
                {
                    for (int y = 0; y < 30; y++)
                    {
                        for (int z = 0; z < 30; z++)
                        {
                            Blocks[x, y, z].Type = BitConverter.ToUInt16(data, index);
                            index += 2;
                        }
                    }
                }
            }
            else
            {
                throw new Exception("Invalid chunk packet size!");
            }
        }

        /// <summary>
        /// Ticks the chunk (and all entities within it).
        /// </summary>
        public void Tick()
        {
            for (int i = 0; i < Tickers.Count; i++)
            {
                Entity e = Tickers[i];
                e.Tick();
                if (!e.IsValid)
                {
                    Tickers.RemoveAt(i);
                    Entities.Remove(e);
                    i--;
                }
            }
        }

        List<ChunkVBO> VBOs = new List<ChunkVBO>();

        public void Render()
        {
            for (int i = 0; i < VBOs.Count; i++)
            {
                VBOs[i].Render();
            }
        }

        ChunkVBO GetVBO(Texture text)
        {
            if (text == null)
            {
                return null;
            }
            for (int i = 0; i < VBOs.Count; i++)
            {
                if (VBOs[i].VBOTexture.Internal_Texture == text.Internal_Texture)
                {
                    return VBOs[i];
                }
            }
            ChunkVBO cvbo = new ChunkVBO();
            cvbo.VBOTexture = text;
            VBOs.Add(cvbo);
            return cvbo;
        }

        /// <summary>
        /// (Re-)Generate the Vertex Buffer Object for this chunk (IE, update its rendered state on the GPU.)
        /// </summary>
        public void UpdateVBO()
        {
            TriangleMesh mesh = new TriangleMesh(false, false);
            for (int i = 0; i < VBOs.Count; i++)
            {
                VBOs[i].Destroy();
            }
            VBOs.Clear();
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    for (int z = 0; z < 30; z++)
                    {
                        MaterialSideData tex = MaterialTexture.GetTexture((Material)Blocks[x, y, z].Type);
                        if (tex != null)
                        {
                            // TODO: Simplify. Can this be a loop?
                            if (z == 29 || !((Material)Blocks[x, y, z + 1].Type).OccupiesWholeBlock())
                            {
                                GetVBO(tex.Textures[(int)Sides.TOP]).AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new OpenTK.Vector3(0, 0, 1));
                            }
                            if (z == 0 || !((Material)Blocks[x, y, z - 1].Type).OccupiesWholeBlock())
                            {
                                GetVBO(tex.Textures[(int)Sides.BOTTOM]).AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new OpenTK.Vector3(0, 0, -1));
                            }
                            if (x == 29 || !((Material)Blocks[x + 1, y, z].Type).OccupiesWholeBlock())
                            {
                                GetVBO(tex.Textures[(int)Sides.XP]).AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new OpenTK.Vector3(1, 0, 0));
                            }
                            if (x == 0 || !((Material)Blocks[x - 1, y, z].Type).OccupiesWholeBlock())
                            {
                                GetVBO(tex.Textures[(int)Sides.XM]).AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new OpenTK.Vector3(-1, 0, 0));
                            }
                            if (y == 29 || !((Material)Blocks[x, y + 1, z].Type).OccupiesWholeBlock())
                            {
                                GetVBO(tex.Textures[(int)Sides.YP]).AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new OpenTK.Vector3(0, 1, 0));
                            }
                            if (y == 0 || !((Material)Blocks[x, y - 1, z].Type).OccupiesWholeBlock())
                            {
                                GetVBO(tex.Textures[(int)Sides.YM]).AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new OpenTK.Vector3(0, -1, 0));
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < VBOs.Count; i++)
            {
                for (int x = 0; x < VBOs[i].Vecs.Count; x += 4)
                {
                    mesh.AddTriangle(new BulletSharp.Vector3(VBOs[i].Vecs[x].X, VBOs[i].Vecs[x].Y, VBOs[i].Vecs[x].Z),
                        new BulletSharp.Vector3(VBOs[i].Vecs[x + 1].X, VBOs[i].Vecs[x + 1].Y, VBOs[i].Vecs[x + 1].Z),
                        new BulletSharp.Vector3(VBOs[i].Vecs[x + 2].X, VBOs[i].Vecs[x + 2].Y, VBOs[i].Vecs[x + 2].Z));
                    mesh.AddTriangle(new BulletSharp.Vector3(VBOs[i].Vecs[x].X, VBOs[i].Vecs[x].Y, VBOs[i].Vecs[x].Z),
                        new BulletSharp.Vector3(VBOs[i].Vecs[x + 3].X, VBOs[i].Vecs[x + 3].Y, VBOs[i].Vecs[x + 3].Z),
                        new BulletSharp.Vector3(VBOs[i].Vecs[x + 2].X, VBOs[i].Vecs[x + 2].Y, VBOs[i].Vecs[x + 2].Z));
                }
                VBOs[i].Build();
            }
            if (Body != null)
            {
                ClientMain.PhysicsWorld.RemoveRigidBody(Body);
            }
            if (mesh.NumTriangles < 2)
            {
                Body = null;
                return;
            }
            DefaultMotionState body_motion_state = new DefaultMotionState(Matrix.Translation(X * 30, Y * 30, Z * 30));
            RigidBodyConstructionInfo rigid_body_ci;
            BvhTriangleMeshShape trianglemesh = new BvhTriangleMeshShape(mesh, false);
            trianglemesh.BuildOptimizedBvh();
            rigid_body_ci = new RigidBodyConstructionInfo(0f, body_motion_state, trianglemesh);
            rigid_body_ci.Friction = 0.5f;
            Body = new RigidBody(rigid_body_ci);
            Body.WorldTransform = Matrix.Translation(X * 30, Y * 30, Z * 30);
            ClientMain.PhysicsWorld.AddRigidBody(Body);
        }
    }
}