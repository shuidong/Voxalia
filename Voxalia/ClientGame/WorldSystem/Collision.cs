using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.WorldSystem
{
    /// <summary>
    /// Handles all clientside collision calculation.
    /// </summary>
    public class Collision
    {
        static bool IsSolid(Location pos)
        {
            return ClientMain.GetBlock(pos).Type.IsSolid();
        }

        /// <summary>
        /// Determines whether a specific point in the 3D world is solid.
        /// </summary>
        /// <param name="point">The point to check</param>
        /// <returns>Whether the point is solid</returns>
        public static bool Point(Location point)
        {
            return IsSolid(point); // TODO: Specificity
        }

        /// <summary>
        /// Returns whether there is anything solid within a box in 3D space.
        /// </summary>
        /// <param name="min">The lowest location</param>
        /// <param name="max">The highest location</param>
        /// <returns>Whether there is anything solid within the block</returns>
        public static bool Box(Location min, Location max)
        {
            return false; // TODO
        }

        /// <summary>
        /// Returns the closest to the end a ray can get through the physical world.
        /// </summary>
        /// <param name="start">The starting location</param>
        /// <param name="end">The ideal ending location</param>
        /// <returns>The actual ending location of a ray trace</returns>
        public Location RayTrace(Location start, Location end)
        {
            return end; // TODO
        }
    }
}
