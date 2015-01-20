using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Text;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// A class for rendering text within OpenGL.
    /// </summary>
    public class GLFont
    {
        /// <summary>
        /// The default font.
        /// </summary>
        public static GLFont Standard;

        /// <summary>
        /// A full list of loaded GLFonts.
        /// </summary>
        public static List<GLFont> Fonts;

        /// <summary>
        /// Prepares the font system.
        /// </summary>
        public static void Init()
        {
            if (Fonts != null)
            {
                for (int i = 0; i < Fonts.Count; i++)
                {
                    Fonts[i].Remove();
                    i--;
                }
            }
            LoadTextFile();
            Fonts = new List<GLFont>();
            // Choose a default font: Segoe UI, Arial, Calibri, or generic.
            FontFamily[] families = FontFamily.Families;
            FontFamily family = FontFamily.GenericMonospace;
            int family_priority = 0;
            for (int i = 0; i < families.Length; i++)
            {
                if (family_priority < 20 && families[i].Name.ToLower() == "dejavu serif")
                {
                    family = families[i];
                    family_priority = 20;
                }
                else if (family_priority < 10 && families[i].Name.ToLower() == "segoe ui")
                {
                    family = families[i];
                    family_priority = 10;
                }
                else if (family_priority < 5 && families[i].Name.ToLower() == "arial")
                {
                    family = families[i];
                    family_priority = 5;
                }
                else if (family_priority < 2 && families[i].Name.ToLower() == "calibri")
                {
                    family = families[i];
                    family_priority = 2;
                }
            }
            Font def = new Font(family, 12);
            Standard = new GLFont(def);
            Fonts.Add(Standard);
        }

        /// <summary>
        /// The text file string to base letters on.
        /// </summary>
        public static string textfile;

        /// <summary>
        /// Loads the character list file.
        /// </summary>
        public static void LoadTextFile()
        {
            textfile = "";
            string[] datas;
            if (FileHandler.Exists("info/characters.dat"))
            {
                datas = FileHandler.ReadText("info/characters.dat").Replace("\r", "").Split('\n');
            }
            else
            {
                datas = new string[] { " abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*()_+-=~`[]{};:'\",./<>?\\| " };
            }
            for (int i = 0; i < datas.Length; i++)
            {
                if (datas[i].Length > 0 && !datas[i].StartsWith("//"))
                {
                    textfile += datas[i];
                }
            }
            string tempfile = "?";
            for (int i = 0; i < textfile.Length; i++)
            {
                if (!tempfile.Contains(textfile[i]))
                {
                    tempfile += textfile[i].ToString();
                }
            }
            textfile = tempfile;
        }

        /// <summary>
        /// Gets the font matching the specified settings.
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="bold">Whether it's bold</param>
        /// <param name="italic">Whether it's italic</param>
        /// <param name="size">The font size</param>
        /// <returns>A valid font object</returns>
        public static GLFont GetFont(string name, bool bold, bool italic, int size)
        {
            string namelow = name.ToLower();
            for (int i = 0; i < Fonts.Count; i++)
            {
                if (Fonts[i].Name.ToLower() == namelow && bold == Fonts[i].Bold && italic == Fonts[i].Italic && size == Fonts[i].Size)
                {
                    return Fonts[i];
                }
            }
            GLFont Loaded = LoadFont(name, bold, italic, size);
            if (Loaded == null)
            {
                return Standard;
            }
            Fonts.Add(Loaded);
            return Loaded;
        }

        /// <summary>
        /// Loads a font matching the specified settings.
        /// </summary>
        /// <param name="name">The name of the font</param>
        /// <param name="bold">Whether it's bold</param>
        /// <param name="italic">Whether it's italic</param>
        /// <param name="size">The font size</param>
        /// <returns>A valid font object, or null if there was no match</returns>
        public static GLFont LoadFont(string name, bool bold, bool italic, int size)
        {
            Font font = new Font(name, size, (bold ? FontStyle.Bold : 0) | (italic ? FontStyle.Italic : 0));
            return new GLFont(font);
        }

        /// <summary>
        /// The texture containing all character images.
        /// </summary>
        public Texture BaseTexture;

        /// <summary>
        /// A list of all supported characters.
        /// </summary>
        public string Characters;

        /// <summary>
        /// A list of all character locations on the base texture.
        /// </summary>
        public List<RectangleF> CharacterLocations;

        /// <summary>
        /// The name of the font.
        /// </summary>
        public string Name;

        /// <summary>
        /// The size of the font.
        /// </summary>
        public int Size;

        /// <summary>
        /// Whether the font is bold.
        /// </summary>
        public bool Bold;

        /// <summary>
        /// Whether the font is italic.
        /// </summary>
        public bool Italic;

        /// <summary>
        /// The font used to create this GLFont.
        /// </summary>
        public Font Internal_Font;

        /// <summary>
        /// How tall a rendered symbol is.
        /// </summary>
        public float Height;

        public GLFont(Font font)
        {
            GL.PushMatrix();
            GL.PushAttrib(AttribMask.AllAttribBits);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            Name = font.Name;
            Size = (int)font.Size;
            Bold = font.Bold;
            Italic = font.Italic;
            Height = font.Height;
            CharacterLocations = new List<RectangleF>();
            StringFormat sf = new StringFormat(StringFormat.GenericTypographic);
            sf.FormatFlags |= StringFormatFlags.MeasureTrailingSpaces | StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap;
            Internal_Font = font;
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Replace);
            int bwidth = 1024;
            int bheight = 1024;
            Bitmap bmp = new Bitmap(bwidth, bheight);
            BaseTexture = new Texture();
            BaseTexture.Name = "font/" + FileHandler.CleanFileName(font.Name) + "/" + (font.Bold ? "b" : "") + (font.Italic ? "i" : "") + ((int)font.Size);
            GL.GenTextures(1, out BaseTexture.Internal_Texture);
            BaseTexture.Original_InternalID = BaseTexture.Internal_Texture;
            BaseTexture.Width = bmp.Width;
            BaseTexture.Height = bmp.Height;
            Texture.LoadedTextures.Add(BaseTexture);
            BaseTexture.Bind();
            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bwidth, bheight), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bwidth, bheight, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.Finish();
            bmp.UnlockBits(data);
            BaseTexture.Bind();
            Characters = textfile;
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.Clear(Color.Transparent);
                //gfx.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                gfx.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                float X = 6;
                float Y = 0;
                gfx.FillRectangle(new SolidBrush(Color.White), new Rectangle(0, 0, 5, (int)Height));
                Brush brush = new SolidBrush(Color.White);
                for (int i = 0; i < textfile.Length; i++)
                {
                    string chr = textfile[i] == '\t' ? "    " : textfile[i].ToString();
                    float nwidth = gfx.MeasureString(chr, font, new PointF(0, 0), sf).Width;
                    if (X + nwidth >= 1024)
                    {
                        Y += Height + 4;
                        X = 0;
                    }
                    gfx.DrawString(chr, font, brush, new PointF(X, Y), sf);
                    CharacterLocations.Add(new RectangleF(X, Y, nwidth, Height));
                    X += (float)Math.Ceiling(nwidth) + 4;
                }
            }
            data = bmp.LockBits(new Rectangle(0, 0, bwidth, bheight), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bwidth, bheight, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);
            // TODO: MAKE ALL THIS BELOW CLEAR / SIMPLE
            uint texture;
            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);
            BitmapData datax = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, datax.Width, datax.Height, 0,
            OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, datax.Scan0);
            bmp.UnlockBits(datax);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            uint fbo;
            GL.Ext.GenFramebuffers(1, out fbo);
            GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer, fbo);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, texture, 0);
            GL.Ext.FramebufferTexture2D(FramebufferTarget.DrawFramebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, texture, 0);
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            bmp.Dispose();
            Matrix4 modelview = Matrix4.LookAt(0, 0, 1,
                                               0, 0, 0,
                                               0, 1, 0);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref modelview);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, bwidth, bheight, 0, -1, 1);
            GL.Ext.BindFramebuffer(FramebufferTarget.DrawFramebuffer, fbo);
            GL.PushAttrib(AttribMask.ViewportBit);
            GL.Viewport(0, 0, 1024, 1024);
            BaseTexture.Bind();
            Shader.BlackRemoverShader.Bind();
            GL.Color4(Color.Green);
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0f, 0f);
            GL.Vertex2(0, 1024f);
            GL.TexCoord2(1f, 0f);
            GL.Vertex2(1024, 1024f);
            GL.TexCoord2(1f, 1f);
            GL.Vertex2(1024, 0f);
            GL.TexCoord2(0f, 1f);
            GL.Vertex2(0, 0f);
            GL.End();
            GL.PopAttrib();
            GL.Ext.BindFramebuffer(FramebufferTarget.FramebufferExt, 0);
            GL.DeleteTexture(BaseTexture.Internal_Texture);
            GL.DeleteFramebuffer(fbo);
            BaseTexture.Internal_Texture = texture;
            BaseTexture.Original_InternalID = texture;
            GL.PopAttrib();
            GL.PopMatrix();
        }

        /// <summary>
        /// Removes the GLFont.
        /// </summary>
        public void Remove()
        {
            Fonts.Remove(this);
        }

        /// <summary>
        /// Gets the location of a symbol.
        /// </summary>
        /// <param name="symbol">The symbol to find</param>
        /// <returns>A rectangle containing the precise location of a symbol</returns>
        public RectangleF RectForSymbol(char symbol)
        {
            for (int i = 0; i < Characters.Length; i++)
            {
                if (Characters[i] == symbol)
                {
                    return CharacterLocations[i];
                }
            }
            return CharacterLocations[0];
        }

        /// <summary>
        /// Draws a single symbol at a specified location.
        /// </summary>
        /// <param name="symbol">The symbol to draw.</param>
        /// <param name="X">The X location to draw it at</param>
        /// <param name="Y">The Y location to draw it at</param>
        /// <returns>The length of the character in pixels</returns>
        public double DrawSingleCharacter(char symbol, double X, double Y, bool flip)
        {
            RectangleF rec = RectForSymbol(symbol);
            if (flip)
            {
                GL.TexCoord2(rec.X / 1024, rec.Y / 1024);
                GL.Vertex2(X, Y + rec.Height);
                GL.TexCoord2((rec.X + rec.Width) / 1024, rec.Y / 1024);
                GL.Vertex2(X + rec.Width, Y + rec.Height);
                GL.TexCoord2((rec.X + rec.Width) / 1024, (rec.Y + rec.Height) / 1024);
                GL.Vertex2(X + rec.Width, Y);
                GL.TexCoord2(rec.X / 1024, (rec.Y + rec.Height) / 1024);
                GL.Vertex2(X, Y);
            }
            else
            {
                GL.TexCoord2(rec.X / 1024, rec.Y / 1024);
                GL.Vertex2(X, Y);
                GL.TexCoord2((rec.X + rec.Width) / 1024, rec.Y / 1024);
                GL.Vertex2(X + rec.Width, Y);
                GL.TexCoord2((rec.X + rec.Width) / 1024, (rec.Y + rec.Height) / 1024);
                GL.Vertex2(X + rec.Width, Y + rec.Height);
                GL.TexCoord2(rec.X / 1024, (rec.Y + rec.Height) / 1024);
                GL.Vertex2(X, Y + rec.Height);
            }
            return rec.Width;
        }

        /// <summary>
        /// Draws a string at a specified location.
        /// </summary>
        /// <param name="str">The string to draw.</param>
        /// <param name="X">The X location to draw it at</param>
        /// <param name="Y">The Y location to draw it at</param>
        /// <returns>The length of the string in pixels</returns>
        public double DrawString(string str, double X, double Y, bool flip = false)
        {
            double nX = 0;
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == '\n')
                {
                    Y += Height;
                    nX = 0;
                    Console.WriteLine("\n!");
                }
                nX += DrawSingleCharacter(str[i], X + nX, Y, flip);
            }
            return nX;
        }

        /// <summary>
        /// Draws a string, handling all relevant graphics code.
        /// </summary>
        /// <param name="str">The string to draw.</param>
        /// <param name="X">The X location to draw it at</param>
        /// <param name="Y">The Y location to draw it at</param>
        /// <param name="c">The color to draw it in</param>
        public void DrawStringFull(string str, double X, double Y, Color c)
        {
            BaseTexture.Bind();
            Shader.ColorMultShader.Bind();
            GL.Color4(c);
            GL.Begin(PrimitiveType.Quads);
            DrawString(str, X, Y, false);
            GL.End();
        }

        /// <summary>
        /// Measures the drawn length of a string.
        /// </summary>
        /// <param name="str">The string to measure</param>
        /// <returns>The length of the string</returns>
        public float MeasureString(string str)
        {
            float X = 0;
            for (int i = 0; i < str.Length; i++)
            {
                X += RectForSymbol(str[i]).Width;
            }
            return X;
        }
    }
}
