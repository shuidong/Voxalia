﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.Shared;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// Contains various GLFonts needed to render fancy text.
    /// </summary>
    public class FontSet
    {
        /// <summary>
        /// The general font used for all normal purposes.
        /// </summary>
        public static FontSet Standard;

        public static FontSet SlightlyBigger;

        /// <summary>
        /// A list of all currently loaded font sets.
        /// </summary>
        public static List<FontSet> Fonts = new List<FontSet>();

        /// <summary>
        /// Prepares the FontSet system.
        /// </summary>
        public static void Init()
        {
            Standard = new FontSet("standard");
            Standard.Load(GLFont.Standard.Name, GLFont.Standard.Size);
            Fonts.Add(Standard);
            SlightlyBigger = new FontSet("slightlybigger");
            SlightlyBigger.Load(GLFont.Standard.Name, GLFont.Standard.Size + 5);
            Fonts.Add(SlightlyBigger);
        }

        /// <summary>
        /// Gets a font by a specified name.
        /// </summary>
        /// <param name="fontname">The name of the font</param>
        /// <param name="fontsize">The size of the font</param>
        /// <returns>The specified font</returns>
        public static FontSet GetFont(string fontname, int fontsize)
        {
            string namelow = fontname.ToLower();
            for (int i = 0; i < Fonts.Count; i++)
            {
                if (Fonts[i].font_default.Size == fontsize && Fonts[i].Name == namelow)
                {
                    return Fonts[i];
                }
            }
            FontSet toret = new FontSet(fontname);
            toret.Load(fontname, fontsize);
            Fonts.Add(toret);
            return toret;
        }

        public GLFont font_default;
        public GLFont font_bold;
        public GLFont font_italic;
        public GLFont font_bolditalic;
        public GLFont font_half;
        public GLFont font_boldhalf;
        public GLFont font_italichalf;
        public GLFont font_bolditalichalf;

        public string Name;

        public FontSet(string _name)
        {
            Name = _name.ToLower();
        }

        public void Load(string fontname, int fontsize)
        {
            font_default = GLFont.GetFont(fontname, false, false, fontsize);
            font_bold = GLFont.GetFont(fontname, true, false, fontsize);
            font_italic = GLFont.GetFont(fontname, false, true, fontsize);
            font_bolditalic = GLFont.GetFont(fontname, true, true, fontsize);
            font_half = GLFont.GetFont(fontname, false, false, fontsize / 2);
            font_boldhalf = GLFont.GetFont(fontname, true, false, fontsize / 2);
            font_italichalf = GLFont.GetFont(fontname, false, true, fontsize / 2);
            font_bolditalichalf = GLFont.GetFont(fontname, true, true, fontsize / 2);
        }


        public const int DefaultColor = 7;
        public static Color[] colors = new Color[] { 
            Color.FromArgb(0, 0, 0),      // 0  // 0 // Black
            Color.FromArgb(255, 0, 0),    // 1  // 1 // Red
            Color.FromArgb(0,255,0),      // 2  // 2 // Green
            Color.FromArgb(255, 255, 0),  // 3  // 3 // Yellow
            Color.FromArgb(0, 0, 255),    // 4  // 4 // Blue
            Color.FromArgb(0, 255, 255),  // 5  // 5 // Cyan
            Color.FromArgb(255, 0, 255),  // 6  // 6 // Magenta
            Color.FromArgb(255, 255, 255),// 7  // 7 // White
            Color.FromArgb(128,0,255),    // 8  // 8 // Purple
            Color.FromArgb(0, 128, 90),   // 9  // 9 // Torqoise
            Color.FromArgb(122, 77, 35),  // 10 // a // Brown
            Color.FromArgb(128, 0, 0),    // 11 // ! // DarkRed
            Color.FromArgb(0, 128, 0),    // 12 // @ // DarkGreen
            Color.FromArgb(128, 128, 0),  // 13 // # // DarkYellow
            Color.FromArgb(0, 0, 128),    // 14 // $ // DarkBlue
            Color.FromArgb(0, 128, 128),  // 15 // % // DarkCyan
            Color.FromArgb(128, 0, 128),  // 16 // - // DarkMagenta
            Color.FromArgb(128, 128, 128),// 17 // & // LightGray
            Color.FromArgb(64, 0, 128),   // 18 // * // DarkPurple
            Color.FromArgb(0, 64, 40),    // 19 // ( // DarkTorqoise
            Color.FromArgb(64, 64, 64),   // 20 // ) // DarkGray
            Color.FromArgb(61, 38, 17),   // 21 // A // DarkBrown
        };
        public static Point[] ShadowPoints = new Point[] {
            new Point(0, 1),
            new Point(1, 0),
            new Point(1, 1),
        };
        public static Point[] BetterShadowPoints = new Point[] {
            new Point(0, 2),
            new Point(1, 2),
            new Point(2, 0),
            new Point(2, 1),
            new Point(2, 2),
        };
        public static Point[] EmphasisPoints = new Point[] {
            new Point(0, -1),
            new Point(0, 1),
            new Point(1, 0),
            new Point(-1, 0),
        };
        public static Point[] BetterEmphasisPoints = new Point[] {
            new Point(-1, -1),
            new Point(-1, 1),
            new Point(1, -1),
            new Point(1, 1),
            new Point(0, -2),
            new Point(0, 2),
            new Point(2, 0),
            new Point(-2, 0),
        };

        /// <summary>
        /// Correctly forms a Color object for the color number and transparency amount, for use by RenderColoredText
        /// </summary>
        /// <param name="color">The color number</param>
        /// <param name="trans">Transparency value, 0-255</param>
        /// <returns>A correctly formed color object</returns>
        public static Color ColorFor(int color, int trans)
        {
            return Color.FromArgb(trans, colors[color].R, colors[color].G, colors[color].B);
        }

        /// <summary>
        /// Fully renders colorful/fancy text (unless the text is not marked as fancy, or fancy rendering is disabled)
        /// </summary>
        /// <param name="Text">The text to render</param>
        /// <param name="Position">Where to render the text at</param>
        /// <param name="MaxY">The maximum Y location to render text at.</param>
        /// <param name="transmod">Transparency modifier (EG, 0.5 = half opacity) (0.0 - 1.0)</param>
        /// <param name="extrashadow">Whether to always have a mini drop-shadow</param>
        public void DrawColoredText(string Text, Location Position, int MaxY = int.MaxValue, float transmod = 1, bool extrashadow = false)
        {
            string[] lines = Text.Replace('\r', ' ').Replace(' ', (char)0x00A0).Replace("^q", "\"").Split('\n');
            int color = DefaultColor;
            int trans = (int)(255 * transmod);
            bool bold = false;
            bool italic = false;
            bool underline = false;
            bool strike = false;
            bool overline = false;
            bool highlight = false;
            bool emphasis = false;
            int ucolor = DefaultColor;
            int scolor = DefaultColor;
            int ocolor = DefaultColor;
            int hcolor = DefaultColor;
            int ecolor = DefaultColor;
            bool super = false;
            bool sub = false;
            bool flip = false;
            bool pseudo = false;
            bool jello = false;
            bool obfu = false;
            bool random = false;
            bool shadow = false;
            int otrans = (int)(255 * transmod);
            int etrans = (int)(255 * transmod);
            int htrans = (int)(255 * transmod);
            int strans = (int)(255 * transmod);
            int utrans = (int)(255 * transmod);
            double X = Position.X;
            double Y = Position.Y;
            GLFont font = font_default;
            font.BaseTexture.Bind();
            Shader.ColorMultShader.Bind();
            GL.Begin(PrimitiveType.Quads);
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];
                if (line.Length == 0)
                {
                    Y += font_default.Height;
                    continue;
                }
                int start = 0;
                for (int x = 0; x < line.Length; x++)
                {
                    if ((line[x] == '^' && x + 1 < line.Length && IsColorSymbol(line[x + 1])) || (x + 1 == line.Length))
                    {
                        string drawme = line.Substring(start, (x - start) + ((x + 1 < line.Length) ? 0 : 1));
                        start = x + 2;
                        x++;
                        if (drawme.Length > 0 && Y >= -font.Height && Y - (sub ? font.Height : 0) <= MaxY)
                        {
                            float width = font.MeasureString(drawme);
                            if (highlight)
                            {
                                DrawRectangle(X, Y, width, font.Height, ColorFor(hcolor, htrans));
                            }
                            if (underline)
                            {
                                DrawRectangle(X, Y + ((float)font.Height * 4 / 5), width, 1, ColorFor(ucolor, utrans));
                            }
                            if (overline)
                            {
                                DrawRectangle(X, Y + 2, width, 1, ColorFor(ocolor, otrans));
                            }
                            if (extrashadow)
                            {
                                foreach (Point point in ShadowPoints)
                                {
                                    RenderBaseText(X + point.X, Y + point.Y, drawme, font, 0, trans / 2, flip);
                                }
                            }
                            if (shadow)
                            {
                                foreach (Point point in ShadowPoints)
                                {
                                    RenderBaseText(X + point.X, Y + point.Y, drawme, font, 0, 255, flip);
                                }
                                foreach (Point point in BetterShadowPoints)
                                {
                                    RenderBaseText(X + point.X, Y + point.Y, drawme, font, 0, 255, flip);
                                }
                            }
                            if (emphasis)
                            {
                                foreach (Point point in EmphasisPoints)
                                {
                                    RenderBaseText(X + point.X, Y + point.Y, drawme, font, ecolor, etrans, flip);
                                }
                                foreach (Point point in BetterEmphasisPoints)
                                {
                                    RenderBaseText(X + point.X, Y + point.Y, drawme, font, ecolor, etrans, flip);
                                }
                            }
                            RenderBaseText(X, Y, drawme, font, color, trans, flip, pseudo, random, jello, obfu);
                            if (strike)
                            {
                                DrawRectangle(X, Y + (font.Height / 2), width, 1, ColorFor(scolor, strans));
                            }
                            X += width;
                        }
                        if (x < line.Length)
                        {
                            switch (line[x])
                            {
                                case '1': color = 1; break;
                                case '!': color = 11; break;
                                case '2': color = 2; break;
                                case '@': color = 12; break;
                                case '3': color = 3; break;
                                case '#': color = 13; break;
                                case '4': color = 4; break;
                                case '$': color = 14; break;
                                case '5': color = 5; break;
                                case '%': color = 15; break;
                                case '6': color = 6; break;
                                case '-': color = 16; break;
                                case '7': color = 7; break;
                                case '&': color = 17; break;
                                case '8': color = 8; break;
                                case '*': color = 18; break;
                                case '9': color = 9; break;
                                case '(': color = 19; break;
                                case '0': color = 0; break;
                                case ')': color = 20; break;
                                case 'a': color = 10; break;
                                case 'A': color = 21; break;
                                case 'i':
                                    {
                                        italic = true;
                                        GLFont nfont = (super || sub) ? (bold ? font_bolditalichalf : font_italichalf) :
                                            (bold ? font_bolditalic : font_italic);
                                        if (nfont != font)
                                        {
                                            GL.End();
                                            font = nfont;
                                            font.BaseTexture.Bind();
                                            GL.Begin(PrimitiveType.Quads);
                                        }
                                    }
                                    break;
                                case 'b':
                                    {
                                        bold = true;
                                        GLFont nfont = (super || sub) ? (italic ? font_bolditalichalf : font_boldhalf) :
                                            (italic ? font_bolditalic : font_bold);
                                        if (nfont != font)
                                        {
                                            GL.End();
                                            font = nfont;
                                            font.BaseTexture.Bind();
                                            GL.Begin(PrimitiveType.Quads);
                                        }
                                    }
                                    break;
                                case 'u': utrans = trans; underline = true; ucolor = color; break;
                                case 's': strans = trans; strike = true; scolor = color; break;
                                case 'h': htrans = trans; highlight = true; hcolor = color; break;
                                case 'e': etrans = trans; emphasis = true; ecolor = color; break;
                                case 'O': otrans = trans; overline = true; ocolor = color; break;
                                case 't': trans = (int)(128 * transmod); break;
                                case 'T': trans = (int)(64 * transmod); break;
                                case 'o': trans = (int)(255 * transmod); break;
                                case 'S':
                                    if (!super)
                                    {
                                        if (sub)
                                        {
                                            sub = false;
                                            Y -= font.Height / 2;
                                        }
                                        GLFont nfont = bold && italic ? font_bolditalichalf : bold ? font_boldhalf :
                                            italic ? font_italichalf : font_half;
                                        if (nfont != font)
                                        {
                                            GL.End();
                                            font = nfont;
                                            font.BaseTexture.Bind();
                                            GL.Begin(PrimitiveType.Quads);
                                        }
                                    }
                                    super = true;
                                    break;
                                case 'l':
                                    if (!sub)
                                    {
                                        if (super)
                                        {
                                            super = false;
                                        }
                                        Y += font_default.Height / 2;
                                        GLFont nfont = bold && italic ? font_bolditalichalf : bold ? font_boldhalf :
                                            italic ? font_italichalf : font_half;
                                        if (nfont != font)
                                        {
                                            GL.End();
                                            font = nfont;
                                            font.BaseTexture.Bind();
                                            GL.Begin(PrimitiveType.Quads);
                                        }
                                    }
                                    sub = true;
                                    break;
                                case 'd': shadow = true; break;
                                case 'j': jello = true; break;
                                case 'k': obfu = true; break;
                                case 'R': random = true; break;
                                case 'p': pseudo = true; break;
                                case 'f': flip = true; break;
                                case 'n':
                                    break;
                                case 'r':
                                    {
                                        GLFont nfont = font_default;
                                        if (nfont != font)
                                        {
                                            GL.End();
                                            font = nfont;
                                            font.BaseTexture.Bind();
                                            GL.Begin(PrimitiveType.Quads);
                                        }
                                        if (sub)
                                        {
                                            Y -= font_default.Height / 2;
                                        }
                                        sub = false;
                                        super = false;
                                        flip = false;
                                        random = false;
                                        pseudo = false;
                                        jello = false;
                                        obfu = false;
                                        shadow = false;
                                        bold = false;
                                        italic = false;
                                        underline = false;
                                        strike = false;
                                        emphasis = false;
                                        highlight = false;
                                        trans = (int)(255 * transmod);
                                        overline = false;
                                        break;
                                    }
                                default:
                                    break;
                            }
                        }
                    }
                }
                Y += font_default.Height;
                X = Position.X;
            }
            GL.End();
            GL.UseProgram(0);
        }

        /// <summary>
        /// Semi-internal rendering of text strings.
        /// </summary>
        /// <param name="X">The X location to render at</param>
        /// <param name="Y">The Y location to render at</param>
        /// <param name="text">The text to render</param>
        /// <param name="font">The font to use</param>
        /// <param name="color">The color ID number to use</param>
        /// <param name="sf">The format</param>
        /// <param name="trans">Transparency</param>
        /// <param name="flip">Whether to flip the text</param>
        /// <param name="pseudo">Whether to use pseudo-random color</param>
        /// <param name="random">Whether to use real-random color</param>
        /// <param name="jello">Whether to use a jello effect</param>
        /// <param name="obfu">Whether to randomize letters</param>
        /// <returns>The length of the rendered text in pixels</returns>
        public static double RenderBaseText(double X, double Y, string text, GLFont font, int color,
            int trans = 255, bool flip = false, bool pseudo = false, bool random = false, bool jello = false, bool obfu = false)
        {
            if (obfu || pseudo || random || jello)
            {
                float nX = 0;
                for (int z = 0; z < text.Length; z++)
                {
                    char chr = text[z];
                    int col = color;
                    if (pseudo)
                    {
                        GL.Color4(ColorFor((chr % (colors.Length - 1)) + 1, trans));
                    }
                    if (random)
                    {
                        GL.Color4(ColorFor(Utilities.UtilRandom.Next(colors.Length), trans));
                    }
                    if (obfu)
                    {
                        chr = (char)Utilities.UtilRandom.Next(33, 126);
                    }
                    int iX = 0;
                    int iY = 0;
                    if (jello)
                    {
                        iX = Utilities.UtilRandom.Next(-1, 1);
                        iY = Utilities.UtilRandom.Next(-1, 1);
                    }
                    font.DrawSingleCharacter(chr, X + iX + nX, Y + iY, flip);
                    nX += font.RectForSymbol(text[z]).Width;
                }
                return nX;
            }
            else
            {
                GL.Color4(ColorFor(color, trans));
                return font.DrawString(text, X, Y, flip);
            }
        }

        public static Location MeasureFancyLinesOfText(string text, FontSet set)
        {
            string[] data = text.Split('\n');
            float len = 0;
            for (int i = 0; i < data.Length; i++)
            {
                float newlen = MeasureFancyText(data[i], set);
                if (newlen > len)
                {
                    len = newlen;
                }
            }
            return new Location(len, data.Length * set.font_default.Height, 0);
        }

        /// <summary>
        /// Measures fancy notated text strings.
        /// Note: Do not include newlines!
        /// </summary>
        /// <param name="line">The text to measure</param>
        /// <param name="set">The FontSet to get fonts from</param>
        /// <returns>the X-width of the text</returns>
        public static float MeasureFancyText(string line, FontSet set)
        {
            bool bold = false;
            bool italic = false;
            bool sub = false;
            float MeasWidth = 0;
            GLFont font = set.font_default;
            int start = 0;
            line = line.Replace("^q", "\"");
            for (int x = 0; x < line.Length; x++)
            {
                if ((line[x] == '^' && x + 1 < line.Length && IsColorSymbol(line[x + 1])) || (x + 1 == line.Length))
                {
                    string drawme = line.Substring(start, (x - start) + ((x + 1 < line.Length) ? 0 : 1));
                    start = x + 2;
                    x++;
                    if (drawme.Length > 0)
                    {
                        MeasWidth += font.MeasureString(drawme);
                    }
                    if (x < line.Length)
                    {
                        switch (line[x])
                        {
                            case 'r':
                                font = set.font_default;
                                bold = false;
                                sub = false;
                                italic = false;
                                break;
                            case 'S':
                            case 'l':
                                font = bold && italic ? set.font_bolditalichalf : bold ? set.font_boldhalf :
                                    italic ? set.font_italichalf : set.font_half;
                                sub = true;
                                break;
                            case 'i':
                                italic = true;
                                font = (sub) ? (bold ? set.font_bolditalichalf : set.font_italichalf) :
                                    (bold ? set.font_bolditalic : set.font_italic);
                                break;
                            case 'b':
                                bold = true;
                                font = (sub) ? (italic ? set.font_bolditalichalf : set.font_boldhalf) :
                                    (italic ? set.font_bolditalic : set.font_bold);
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
            return MeasWidth;
        }

        /// <summary>
        /// Draws a rectangle to screen.
        /// </summary>
        /// <param name="X">The starting X</param>
        /// <param name="Y">The starting Y</param>
        /// <param name="width">The width</param>
        /// <param name="height">The height</param>
        /// <param name="c">The color to use</param>
        public static void DrawRectangle(double X, double Y, float width, float height, Color c)
        {
            if (c != null) GL.Color4(c);
            GL.TexCoord2(2f / 1024f, 2 / 1024f);
            GL.Vertex2(X, Y);
            GL.TexCoord2(4f / 1024f, 2f / 1024f);
            GL.Vertex2(X + width, Y);
            GL.TexCoord2(4f / 1024f, 4f / 1024f);
            GL.Vertex2(X + width, Y + height);
            GL.TexCoord2(2f / 1024f, 4f / 1024f);
            GL.Vertex2(X, Y + height);
        }

        /// <summary>
        /// Used to identify if an input character is a valid color symbol (generally the character that follows a '^'), for use by RenderColoredText
        /// </summary>
        /// <param name="c"><paramref name="c"/>The character to check</param>
        /// <returns>whether the character is a valid color symbol</returns>
        public static bool IsColorSymbol(char c)
        {
            return ((c >= '0' && c <= '9') /* 0123456789 */ ||
                    (c >= 'a' && c <= 'b') /* ab */ ||
                    (c >= 'd' && c <= 'f') /* def */ ||
                    (c >= 'h' && c <= 'l') /* hijkl */ ||
                    (c >= 'n' && c <= 'u') /* nopqrstu */ ||
                    (c >= 'R' && c <= 'T') /* RST */ ||
                    (c >= '#' && c <= '&') /* #$%& */ || // 35 - 38
                    (c >= '(' && c <= '*') /* ()* */ || // 40 - 42
                    (c == 'A') ||
                    (c == 'O') ||
                    (c == '-') || // 45
                    (c == '!') || // 33
                    (c == '@')    // 64
                   );
        }
    }
}
