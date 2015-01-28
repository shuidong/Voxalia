using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.GraphicsSystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.EntitySystem
{
    /// <summary>
    /// Represents an entity designed by the serverside.
    /// </summary>
    public class ServerEntity: Entity
    {
        /// <summary>
        /// Constructs an entity that was sent by the server.
        /// Fields must be filled after creating.
        /// </summary>
        public ServerEntity()
            : base(true)
        {
        }

        /// <summary>
        /// The texture this entity's model wears.
        /// </summary>
        public Texture EntTexture;

        /// <summary>
        /// The model this entity wears.
        /// </summary>
        public Model EntModel;

        /// <summary>
        /// The color of this entity.
        /// </summary>
        public Color4 Color;

        /// <summary>
        /// The scale of this entity's model.
        /// </summary>
        public Location Scale;

        /// <summary>
        /// Converts an unsigned integer to a color.
        /// </summary>
        /// <param name="color">The color int</param>
        /// <returns>A color object</returns>
        public static Color4 IntToColor(uint color)
        {
            return new Color4((byte)(color), (byte)(color >> 8), (byte)(color >> 16), (byte)(color >> 24));
        }

        public override void Tick()
        {
            Position += Velocity * ClientMain.Delta;
        }

        public override void Render3D()
        {
            GL.Color4(Color);
            EntTexture.Bind();
            GL.PushMatrix();
            GL.Translate(Position.ToOVector());
            GL.Scale(Scale.ToOVector());
            GL.Rotate(Direction.X, Vector3d.UnitZ);
            GL.Rotate(Direction.Y, Vector3d.UnitY);
            GL.Rotate(Direction.Z, Vector3d.UnitX);
            EntModel.Draw();
            GL.PopMatrix();
        }
    }
}
