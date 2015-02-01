using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ServerGame.WorldSystem
{
    /// <summary>
    /// Represents a holdable item.
    /// </summary>
    public class Item
    {
        /// <summary>
        /// The name of the item.
        /// </summary>
        public string Name;

        /// <summary>
        /// The texture of the item.
        /// </summary>
        public string Texture;

        /// <summary>
        /// The shader used to render the item.
        /// </summary>
        public string Shader;

        /// <summary>
        /// How many of the item there are.
        /// </summary>
        public int Quantity;

        /// <summary>
        /// What color this item is.
        /// </summary>
        public uint Color = uint.MaxValue;

        /// <summary>
        /// A description of the item.
        /// </summary>
        public string Description;

        /// <summary>
        /// What material this item would place.
        /// </summary>
        public Material Material = Material.AIR;
    }
}
