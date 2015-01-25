using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.WorldSystem;
using Voxalia.Shared;

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
        public Location Position;

        /// <summary>
        /// What direction this entity is facing.
        /// X = Yaw,
        /// Y = Pitch,
        /// Z = Roll.
        /// </summary>
        public Location Direction;

        /// <summary>
        /// The movement velocity of this entity.
        /// </summary>
        public Location Velocity;

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
            }
            Position = pos;
        }

        /// <summary>
        /// Freezes the entity.
        /// </summary>
        public virtual void Freeze()
        {
        }

        /// <summary>
        /// Unfreezes the entity.
        /// </summary>
        public virtual void Unfreeze()
        {
        }
    }
}
