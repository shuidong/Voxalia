using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ClientGame.WorldSystem
{
    /// <summary>
    /// Represents a block (does not contain raw data).
    /// Does not neccessarily match the current block at its location.
    /// </summary>
    public class Block
    {
        /// <summary>
        /// The chunk that contains this block.
        /// </summary>
        public Chunk OwningChunk;

        /// <summary>
        /// The X coordinate, relative to the chunk location.
        /// </summary>
        public int RelativeX;

        /// <summary>
        /// The Y coordinate, relative to the chunk location.
        /// </summary>
        public int RelativeY;

        /// <summary>
        /// The Z coordinate, relative to the chunk location.
        /// </summary>
        public int RelativeZ;

        /// <summary>
        /// The material type of the block.
        /// </summary>
        public Material Type;

        /// <summary>
        /// Constructs a new Block representative object.
        /// </summary>
        /// <param name="chunk">The chunk the block is in</param>
        /// <param name="relX">The X coordinate, relative to the chunk location</param>
        /// <param name="relY">The Y coordinate, relative to the chunk location</param>
        /// <param name="relZ">The Z coordinate, relative to the chunk location</param>
        public Block(Chunk chunk, int relX, int relY, int relZ)
        {
            OwningChunk = chunk;
            RelativeX = relX;
            RelativeY = relY;
            RelativeZ = relZ;
            Refresh();
        }

        /// <summary>
        /// Update this block's information based on the actual block's data.
        /// </summary>
        public void Refresh()
        {
            Type = (Material)OwningChunk.Blocks[RelativeX, RelativeY, RelativeZ].Type;
        }

        /// <summary>
        /// Sets the chunk's internal representation of the block to match this block data.
        /// </summary>
        public void Set()
        {
            OwningChunk.Blocks[RelativeX, RelativeY, RelativeZ].Type = (ushort)Type;
        }
    }
}
