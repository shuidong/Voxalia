using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxalia.Shared
{
    /// <summary>
    /// Represents a material.
    /// </summary>
    public enum Material: ushort
    {
        /// <summary>Air (empty block) = 0.</summary>
        AIR = 0,
        /// <summary>Stone block = 1.</summary>
        STONE = 1,
        /// <summary>Dirt block = 2.</summary>
        DIRT = 2,
        /// <summary>Grass block = 3.</summary>
        GRASS = 3,
        /// <summary>Wood block = 4.</summary>
        WOOD = 4,
        /// <summary>Redstone block = 5.</summary>
        REDSTONE = 5,
        /// <summary>How many materials there are by default (6 currently)</summary>
        MAX = 6
    }

    public static class MaterialExtensions
    {
        /// <summary>
        /// Returns whether the material occupies the entire block.
        /// </summary>
        /// <param name="mat">The material</param>
        /// <returns>Whether it occupies the whole block</returns>
        public static bool OccupiesWholeBlock(this Material mat)
        {
            switch (mat)
            {
                case Material.AIR:
                    return false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// Returns whether a block of the material is solid.
        /// </summary>
        /// <param name="mat">The material</param>
        /// <returns>Whether it is solid</returns>
        public static bool IsSolid(this Material mat)
        {
            switch (mat)
            {
                case Material.AIR:
                    return false;
                default:
                    return true;
            }
        }
    }
}
