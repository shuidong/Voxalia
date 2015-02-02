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
        public static Dictionary<ushort, MaterialSideData> Textures;

        static MaterialSideData[] TexturesDefault = new MaterialSideData[(int)Material.MAX];

        /// <summary>
        /// Prepares the system, registering built-in material textures.
        /// </summary>
        public static void Init()
        {
            Textures = new Dictionary<ushort, MaterialSideData>();
            TexturesDefault[0] = null;
            TexturesDefault[1] = new MaterialSideData(Texture.GetTexture("blocks/solid/stone"));
            TexturesDefault[2] = new MaterialSideData(Texture.GetTexture("blocks/solid/dirt"));
            MaterialSideData grass = new MaterialSideData(Texture.GetTexture("blocks/solid/grass_side"));
            grass.Textures[(int)Sides.TOP] = Texture.GetTexture("blocks/solid/grass");
            grass.Textures[(int)Sides.BOTTOM] = Texture.GetTexture("blocks/solid/dirt");
            TexturesDefault[3] = grass;
            TexturesDefault[4] = new MaterialSideData(Texture.GetTexture("blocks/solid/wood"));
            TexturesDefault[5] = new MaterialSideData(Texture.GetTexture("blocks/solid/redstone"));
        }

        public static MaterialSideData GetTexture(Material mat)
        {
            if (mat < Material.MAX)
            {
                return TexturesDefault[(int)mat];
            }
            MaterialSideData text;
            if (Textures.TryGetValue((ushort)mat, out text))
            {
                return text;
            }
            return null;
        }
    }

    /// <summary>
    /// Represents data on a block's texture when it has custom sides.
    /// </summary>
    public class MaterialSideData
    {
        /// <summary>
        /// The textures, indexed by the enum Sides.
        /// </summary>
        public List<Texture> Textures = new List<Texture>();

        public MaterialSideData(Texture def)
        {
            for (int i = 0; i < 6; i++)
            {
                Textures.Add(def);
            }
        }
    }

    /// <summary>
    /// Represents the different sides of a block.
    /// </summary>
    public enum Sides: byte
    {
        TOP = 0,
        BOTTOM = 1,
        XP = 2,
        XM = 3,
        YP = 4,
        YM = 5
    }
}
