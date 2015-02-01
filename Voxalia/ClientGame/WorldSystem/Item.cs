using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.GraphicsSystem;
using OpenTK;
using OpenTK.Graphics;

namespace Voxalia.ClientGame.WorldSystem
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
        /// The texture used by the item.
        /// </summary>
        public Texture Image;

        /// <summary>
        /// The shader used when rendering the item.
        /// </summary>
        public Shader ImageModifier;

        /// <summary>
        /// The color of the item.
        /// </summary>
        public Color4 Color;

        /// <summary>
        /// A description of the item.
        /// </summary>
        public string Description;
    }
}
