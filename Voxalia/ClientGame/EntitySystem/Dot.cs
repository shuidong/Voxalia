using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.EntitySystem
{
    public class Dot: Entity
    {
        public Dot()
            : base(false)
        {
        }

        public override void Tick()
        {
            throw new NotImplementedException();
        }

        public override void Render3D()
        {
            UIConsole.ConsoleTexture.Bind();
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex3(Position.X, Position.Y, Position.Z);
            GL.TexCoord2(0, 1);
            GL.Vertex3(Position.X, Position.Y + 0.01, Position.Z);
            GL.TexCoord2(1, 1);
            GL.Vertex3(Position.X + 0.01, Position.Y + 0.01, Position.Z);
            GL.TexCoord2(1, 0);
            GL.Vertex3(Position.X + 0.01, Position.Y, Position.Z);
            GL.End();
        }
    }
}
