using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.WorldSystem;
using Voxalia.Shared;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;

namespace Voxalia.ServerGame.EntitySystem
{
    /// <summary>
    /// Represents a dynamic entity within a world.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// The world this entity is in.
        /// </summary>
        public World InWorld;

        /// <summary>
        /// Where in the world this entity is in.
        /// </summary>
        public Location Position = Location.Zero;

        /// <summary>
        /// What direction this entity is facing.
        /// X = Yaw,
        /// Y = Pitch,
        /// Z = Roll.
        /// </summary>
        public Location Direction = Location.Zero;

        /// <summary>
        /// The movement velocity of this entity.
        /// </summary>
        public Location Velocity = Location.Zero;

        /// <summary>
        /// The name of this entity's model.
        /// </summary>
        public string Model = "null";

        /// <summary>
        /// The name of this entity's texture.
        /// </summary>
        public string Texture = "null";

        /// <summary>
        /// The color of this entity. (RGBA)
        /// </summary>
        public uint Color = GenerateColor(255, 255, 255, 255);

        /// <summary>
        /// The rendered scale of this entity.
        /// </summary>
        public Location Scale = Location.One;

        /// <summary>
        /// Generates a color int for the RGBA values.
        /// </summary>
        /// <param name="red">The red value</param>
        /// <param name="green">The green value</param>
        /// <param name="blue">The blue value</param>
        /// <param name="alpha">The alpha value</param>
        /// <returns>The color int</returns>
        public static uint GenerateColor(byte red, byte green, byte blue, byte alpha)
        {
            return (uint)(red | (green << 8) | (blue << 16) | (alpha << 24));
        }

        /// <summary>
        /// Whether this entity ticks.
        /// </summary>
        public readonly bool TickMe;

        /// <summary>
        /// Constructs an entity.
        /// </summary>
        /// <param name="ticks">Whether this entity ticks</param>
        public Entity(bool ticks)
        {
            TickMe = ticks;
        }

        /// <summary>
        /// Ticks the entity, as specified by the entity's type.
        /// </summary>
        public abstract void Tick();

        /// <summary>
        /// Whether the entity is still existent.
        /// </summary>
        public bool IsValid = true;

        /// <summary>
        /// This entity's ID. Unique in the world.
        /// </summary>
        public ulong ID;

        /// <summary>
        /// Change the position of the entity, updating it's data.
        /// </summary>
        /// <param name="pos">The new position</param>
        public virtual void Reposition(Location pos)
        {
            Location chunkloc = World.GetChunkLocation(Position);
            Location chunkloc2 = World.GetChunkLocation(pos);
            if (chunkloc2 != chunkloc)
            {
                InWorld.Remove(this);
                InWorld.Spawn(this);
                for (int i = 0; i < InWorld.Players.Count; i++)
                {
                    if (InWorld.Players[i] != this)
                    {
                        bool has_old = InWorld.Players[i].ChunksAware.Contains(chunkloc);
                        bool has_new = InWorld.Players[i].ChunksAware.Contains(chunkloc2);
                        if (has_old && !has_new)
                        {
                            InWorld.Players[i].Send(new DespawnPacketOut(this));
                        }
                        else if (has_new && !has_old)
                        {
                            InWorld.Players[i].Send(new NewEntityPacketOut(this));
                        }
                        if (has_new && has_old)
                        {
                            InWorld.Players[i].Send(new EntityPositionPacketOut(this));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < InWorld.Players.Count; i++)
                {
                    if (InWorld.Players[i] != this)
                    {
                        bool has = InWorld.Players[i].ChunksAware.Contains(chunkloc);
                        if (has && has)
                        {
                            InWorld.Players[i].Send(new EntityPositionPacketOut(this));
                        }
                    }
                }
            }
            Position = pos;
        }
    }
}
