using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.Shared;
using System.Drawing;
using System.Drawing.Imaging;
using Frenetic;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// Wraps an OpenGL texture.
    /// </summary>
    public class Texture
    {
        /// <summary>
        /// A full list of currently loaded textures.
        /// </summary>
        public static List<Texture> LoadedTextures = null;

        /// <summary>
        /// A default white texture.
        /// </summary>
        public static Texture White = null;

        /// <summary>
        /// A default clear texture.
        /// </summary>
        public static Texture Clear = null;

        /// <summary>
        /// A default black texture.
        /// </summary>
        public static Texture Black = null;

        /// <summary>
        /// An empty bitmap, for regular use.
        /// </summary>
        public static Bitmap EmptyBitmap = null;

        /// <summary>
        /// A single graphics object for regular use.
        /// </summary>
        public static Graphics GenericGraphicsObject = null;

        /// <summary>
        /// The currently bound texture object.
        /// </summary>
        public static Texture BoundText = null;

        /// <summary>
        /// Starts or restarts the texture system.
        /// </summary>
        public static void InitTextureSystem()
        {
            // Dispose existing textures
            if (LoadedTextures != null)
            {
                for (int i = 0; i < LoadedTextures.Count; i++)
                {
                    LoadedTextures[i].Remove();
                    i--;
                }
            }
            // Create a generic graphics object for later use
            EmptyBitmap = new Bitmap(1, 1);
            GenericGraphicsObject = Graphics.FromImage(EmptyBitmap);
            // Reset texture list
            LoadedTextures = new List<Texture>();
            // Pregenerate a few needed textures
            White = GenerateForColor(Color.White, "white");
            LoadedTextures.Add(White);
            Black = GenerateForColor(Color.Black, "black");
            LoadedTextures.Add(Black);
            Clear = GenerateForColor(Color.Transparent, "clear");
            LoadedTextures.Add(Clear);
        }

        /// <summary>
        /// Gets the texture object for a specific texture name.
        /// </summary>
        /// <param name="texturename">The name of the texture</param>
        /// <returns>A valid texture object</returns>
        public static Texture GetTexture(string texturename)
        {
            texturename = FileHandler.CleanFileName(texturename);
            for (int i = 0; i < LoadedTextures.Count; i++)
            {
                if (LoadedTextures[i].Name == texturename)
                {
                    return LoadedTextures[i];
                }
            }
            Texture Loaded = LoadTexture(texturename);
            if (Loaded == null)
            {
                Loaded = new Texture();
                Loaded.Name = texturename;
                Loaded.Internal_Texture = White.Original_InternalID;
                Loaded.Original_InternalID = White.Original_InternalID;
                Loaded.LoadedProperly = false;
            }
            LoadedTextures.Add(Loaded);
            return Loaded;
        }

        /// <summary>
        /// Loads a texture from file.
        /// </summary>
        /// <param name="filename">The name of the file to use</param>
        /// <returns>The loaded texture, or null if it does not exist</returns>
        private static Texture LoadTexture(string filename)
        {
            try
            {
                filename = FileHandler.CleanFileName(filename);
                if (!FileHandler.Exists("textures/" + filename + ".png"))
                {
                    SysConsole.Output(OutputType.ERROR, "Cannot load texture, file '" +
                        TextStyle.Color_Standout + "textures/" + filename + ".png" + TextStyle.Color_Error +
                        "' does not exist.");
                    return null;
                }
                Bitmap bmp = new Bitmap(FileHandler.ReadToStream("textures/" + filename + ".png"));
                Texture texture = new Texture();
                texture.Name = filename;
                GL.GenTextures(1, out texture.Original_InternalID);
                texture.Internal_Texture = texture.Original_InternalID;
                texture.Bind();
                LockBitmapToTexture(bmp);
                texture.Width = bmp.Width;
                texture.Height = bmp.Height;
                bmp.Dispose();
                texture.LoadedProperly = true;
                return texture;
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Failed to load texture from filename '" +
                    TextStyle.Color_Standout + "textures/" + filename + ".png" + TextStyle.Color_Error + "': " + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// Creates a Texture object for a specific color.
        /// </summary>
        /// <param name="c">The color to use</param>
        /// <param name="name">The name of the texture</param>
        /// <returns>The generated texture</returns>
        public static Texture GenerateForColor(Color c, string name)
        {
            Texture texture = new Texture();
            texture.Name = name;
            GL.GenTextures(1, out texture.Original_InternalID);
            texture.Internal_Texture = texture.Original_InternalID;
            texture.Bind();
            texture.LoadedProperly = true;
            texture.Width = 2;
            texture.Height = 2;
            Bitmap bmp = new Bitmap(2, 2);
            bmp.SetPixel(0, 0, c);
            bmp.SetPixel(0, 1, c);
            bmp.SetPixel(1, 0, c);
            bmp.SetPixel(1, 1, c);
            LockBitmapToTexture(bmp);
            bmp.Dispose();
            return texture;
        }

        /// <summary>
        /// Locks a bitmap file's data to a GL texture.
        /// </summary>
        /// <param name="bmp">The bitmap to use</param>
        public static void LockBitmapToTexture(Bitmap bmp)
        {
            // Send the bits across
            BitmapData bmp_data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0);
            bmp.UnlockBits(bmp_data);
            // Disable mipmapping
            // TODO: Enable mipmapping optionally?
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
        }

        /// <summary>
        /// The full name of the texture.
        /// </summary>
        public string Name;

        /// <summary>
        /// The texture that this texture was remapped to, if any.
        /// </summary>
        public Texture RemappedTo;

        /// <summary>
        /// The internal OpenGL texture ID.
        /// </summary>
        public uint Internal_Texture = 0;

        /// <summary>
        /// The original OpenGL texture ID that formed this texture.
        /// </summary>
        public uint Original_InternalID = 0;

        /// <summary>
        /// Whether the texture loaded properly.
        /// </summary>
        public bool LoadedProperly = false;

        /// <summary>
        /// The width of the texture.
        /// </summary>
        public int Width;

        /// <summary>
        /// The height of the texture.
        /// </summary>
        public int Height;

        /// <summary>
        /// Removes the texture from the system.
        /// </summary>
        public void Remove()
        {
            if (GL.IsTexture(Original_InternalID))
            {
                GL.DeleteTexture(Original_InternalID);
            }
            LoadedTextures.Remove(this);
        }

        /// <summary>
        /// Saves the texture to a bitmap.
        /// </summary>
        /// <param name="flip">Whether to flip the Y</param>
        public Bitmap SaveToBMP(bool flip = false)
        {
            GL.BindTexture(TextureTarget.Texture2D, Original_InternalID);
            Bitmap bmp = new Bitmap(Width, Height);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.GetTexImage(TextureTarget.Texture2D, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            if (flip)
            {
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            return bmp;
        }

        /// <summary>
        /// Binds this texture to OpenGL.
        /// </summary>
        public void Bind()
        {
            BoundText = this;
            GL.BindTexture(TextureTarget.Texture2D, Internal_Texture);
        }
    }
}
