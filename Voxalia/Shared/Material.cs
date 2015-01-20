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
        WOOD = 4
    }
}
