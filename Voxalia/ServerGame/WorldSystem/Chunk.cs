using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.Shared;
using BulletSharp;

namespace Voxalia.ServerGame.WorldSystem
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
        /// The world that owns this chunk.
        /// </summary>
        public World OwningWorld;

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
        /// <param name="world">The world that owns this new chunk</param>
        public Chunk(int x, int y, int z, World world)
        {
            X = x;
            Y = y;
            Z = z;
            OwningWorld = world;
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

        /// <summary>
        /// Convert the chunk to a byte array, for network transmission.
        /// </summary>
        /// <returns>The byte array</returns>
        public byte[] GetArray()
        {
            byte[] data = new byte[30 * 30 * 30 * 2 + 12]; // 54,000 bytes = ~54 KB
            new Location(X, Y, Z).ToBytes().CopyTo(data, 0);
            int index = 12;
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    for (int z = 0; z < 30; z++)
                    {
                        BitConverter.GetBytes(Blocks[x, y, z].Type).CopyTo(data, index);
                        index += 2;
                    }
                }
            }
            return FileHandler.GZip(data);
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

        /// <summary>
        /// Updates the chunks physical body.
        /// </summary>
        public void UpdateBody()
        {
            TriangleMesh mesh = new TriangleMesh(false, false);
            ChunkVertexHolder cvh = new ChunkVertexHolder();
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    for (int z = 0; z < 30; z++)
                    {
                        if (((Material)Blocks[x, y, z].Type).OccupiesWholeBlock())
                        {
                            // TODO: Simplify. Can this be a loop?
                            if (z == 29 || !((Material)Blocks[x, y, z + 1].Type).OccupiesWholeBlock())
                            {
                                cvh.AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new Vector3(0, 0, 1));
                            }
                            if (z == 0 || !((Material)Blocks[x, y, z - 1].Type).OccupiesWholeBlock())
                            {
                                cvh.AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new Vector3(0, 0, -1));
                            }
                            if (x == 29 || !((Material)Blocks[x + 1, y, z].Type).OccupiesWholeBlock())
                            {
                                cvh.AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new Vector3(1, 0, 0));
                            }
                            if (x == 0 || !((Material)Blocks[x - 1, y, z].Type).OccupiesWholeBlock())
                            {
                                cvh.AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new Vector3(-1, 0, 0));
                            }
                            if (y == 29 || !((Material)Blocks[x, y + 1, z].Type).OccupiesWholeBlock())
                            {
                                cvh.AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new Vector3(0, 1, 0));
                            }
                            if (y == 0 || !((Material)Blocks[x, y - 1, z].Type).OccupiesWholeBlock())
                            {
                                cvh.AddSide(X * 30 + x, Y * 30 + y, Z * 30 + z, new Vector3(0, -1, 0));
                            }
                        }
                    }
                }
            }
            for (int x = 0; x < cvh.Vecs.Count; x += 4)
            {
                mesh.AddTriangle(new BulletSharp.Vector3(cvh.Vecs[x].X, cvh.Vecs[x].Y, cvh.Vecs[x].Z),
                    new BulletSharp.Vector3(cvh.Vecs[x + 1].X, cvh.Vecs[x + 1].Y, cvh.Vecs[x + 1].Z),
                    new BulletSharp.Vector3(cvh.Vecs[x + 2].X, cvh.Vecs[x + 2].Y, cvh.Vecs[x + 2].Z));
                mesh.AddTriangle(new BulletSharp.Vector3(cvh.Vecs[x].X, cvh.Vecs[x].Y, cvh.Vecs[x].Z),
                    new BulletSharp.Vector3(cvh.Vecs[x + 3].X, cvh.Vecs[x + 3].Y, cvh.Vecs[x + 3].Z),
                    new BulletSharp.Vector3(cvh.Vecs[x + 2].X, cvh.Vecs[x + 2].Y, cvh.Vecs[x + 2].Z));
            }
            DefaultMotionState body_motion_state = new DefaultMotionState(Matrix.Translation(0, 0, 0));
            RigidBodyConstructionInfo rigid_body_ci;
            if (Body != null)
            {
                OwningWorld.PhysicsWorld.RemoveRigidBody(Body);
            }
            if (mesh.NumTriangles < 2)
            {
                Body = null;
                return;
            }
            BvhTriangleMeshShape trianglemesh = new BvhTriangleMeshShape(mesh, false);
            trianglemesh.BuildOptimizedBvh();
            rigid_body_ci = new RigidBodyConstructionInfo(0f, body_motion_state, trianglemesh);
            rigid_body_ci.Friction = 0.5f;
            Body = new RigidBody(rigid_body_ci);
            Body.WorldTransform = Matrix.Translation(0, 0, 0);
            OwningWorld.PhysicsWorld.AddRigidBody(Body);
        }

        public override int GetHashCode()
        {
            return X + Y + Z;
        }

        public override bool Equals(object obj)
        {
            return obj is Chunk && ((Chunk)obj).X == X && ((Chunk)obj).Y == Y && ((Chunk)obj).Z == Z;
        }
    }

    /// <summary>
    /// Temporary holder class.
    /// TODO: Factor out
    /// </summary>
    class ChunkVertexHolder
    {
        public List<Vector3> Vecs = new List<Vector3>();
        public void AddSide(float x, float y, float z, Vector3 Normal)
        {
            // TODO: Simplify
            if (Normal.Z == -1)
            {
                Vecs.Add(new Vector3(x, y, z));
                Vecs.Add(new Vector3(x + 1, y, z));
                Vecs.Add(new Vector3(x + 1, y + 1, z));
                Vecs.Add(new Vector3(x, y + 1, z));
            }
            else if (Normal.Z == 1)
            {
                Vecs.Add(new Vector3(x, y, z + 1));
                Vecs.Add(new Vector3(x, y + 1, z + 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z + 1));
                Vecs.Add(new Vector3(x + 1, y, z + 1));
            }
            else if (Normal.X == -1)
            {
                Vecs.Add(new Vector3(x, y, z));
                Vecs.Add(new Vector3(x, y + 1, z));
                Vecs.Add(new Vector3(x, y + 1, z + 1));
                Vecs.Add(new Vector3(x, y, z + 1));
            }
            else if (Normal.X == 1)
            {
                Vecs.Add(new Vector3(x + 1, y, z));
                Vecs.Add(new Vector3(x + 1, y, z + 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z + 1));
                Vecs.Add(new Vector3(x + 1, y + 1, z));
            }
            else if (Normal.Y == -1)
            {
                Vecs.Add(new Vector3(x, y, z));
                Vecs.Add(new Vector3(x, y, z + 1));
                Vecs.Add(new Vector3(x + 1, y, z + 1));
                Vecs.Add(new Vector3(x + 1, y, z));
            }
            else if (Normal.Y == 1)
            {
                Vecs.Add(new Vector3(x, y + 1, z));
                Vecs.Add(new Vector3(x + 1, y + 1, z));
                Vecs.Add(new Vector3(x + 1, y + 1, z + 1));
                Vecs.Add(new Vector3(x, y + 1, z + 1));
            }
        }

    }
}
