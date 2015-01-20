using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ClientGame.EntitySystem
{
    /// <summary>
    /// Represents a dynamic entity within a world.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Where in the world this entity is in.
        /// </summary>
        public Location Position;

        /// <summary>
        /// What direction this entity is facing.
        /// X = Yaw,
        /// Y = Pitch,
        /// Z = Roll.
        /// </summary>
        public Location Direction;

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
    }
}
