using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.EntitySystem;

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
    }
}