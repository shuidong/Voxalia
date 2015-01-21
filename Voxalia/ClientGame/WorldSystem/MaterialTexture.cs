using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.GraphicsSystem;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.WorldSystem
{
    /// <summary>
    /// A helper class to get the texture associated with a material.
    /// </summary>
    public class MaterialTexture
    {
        public static Dictionary<ushort, Texture> Textures;

        /// <summary>
        /// Prepares the system, registering built-in material textures.
        /// </summary>
        public static void Init()
        {
            Textures = new Dictionary<ushort, Texture>();
            Textures.Add(0, null);
            Textures.Add(1, Texture.GetTexture("blocks/solid/stone"));
            Textures.Add(2, Texture.GetTexture("blocks/solid/dirt"));
            Textures.Add(3, Texture.GetTexture("blocks/solid/grass"));
            Textures.Add(4, Texture.GetTexture("blocks/solid/wood"));
        }

        public static Texture GetTexture(Material mat)
        {
            Texture text;
            if (Textures.TryGetValue((ushort)mat, out text))
            {
                return text;
            }
            return UIConsole.ConsoleTexture; // TODO
        }
    }
}
