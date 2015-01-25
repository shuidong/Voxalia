using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ServerGame.ServerMainSystem;

namespace Voxalia.ServerGame.WorldSystem
{
    /// <summary>
    /// Handles all clientside collision calculation.
    /// </summary>
    public class Collision
    {
        static bool IsSolid(World world, Location pos)
        {
            return world.GetBlock(pos).Type.OccupiesWholeBlock(); // TODO: Non-block-filling blocks
        }

        /// <summary>
        /// Determines whether a specific point in the 3D world is solid.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <param name="world">The world the point is in</param>
        /// <returns>Whether the point is solid</returns>
        public static bool Point(World world, Location point)
        {
            return IsSolid(world, point);
        }

        /// <summary>
        /// Returns whether there is anything solid within a box in 3D space.
        /// </summary>
        /// <param name="min">The lowest location</param>
        /// <param name="max">The highest location</param>
        /// <param name="world">The world everything is in</param>
        /// <returns>Whether there is anything solid within the block</returns>
        public static bool Box(World world, Location min, Location max)
        {
            return false; // TODO
        }

        /// <summary>
        /// Returns the closest to the end a ray can get through the physical world.
        /// </summary>
        /// <param name="start">The starting location</param>
        /// <param name="end">The ideal ending location</param>
        /// <param name="world">The world everything is in</param>
        /// <returns>The actual ending location of a ray trace</returns>
        public static Location RayTrace(World world, Location start, Location end, bool bounceback = false)
        {
            return end; // TODO
        }

        /// <summary>
        /// Returns the closest to the end a box-ray can get through the physical world.
        /// </summary>
        /// <param name="start">The starting location</param>
        /// <param name="end">The ideal ending location</param>
        /// <param name="mins">The lower part of the box</param>
        /// <param name="maxes">The higher part of the box</param>
        /// <param name="world">The world everything is in</param>
        /// <param name="bounceback">Whether to jump the collision point back a little</param>
        /// <returns>The actual ending location of a ray trace</returns>
        public static Location BoxRayTrace(World world, Location mins, Location maxes, Location start, Location end, bool bounceback = false)
        {
            return end; // TODO
        }
    }
}
