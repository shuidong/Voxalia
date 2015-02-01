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
            foreach (KeyValuePair<Location, Chunk> chunk in world.LoadedChunks)
            {
                Location cpos = new Location(chunk.Value.X * 30, chunk.Value.Y * 30, chunk.Value.Z * 30);
                if (CollisionUtil.BoxContains(cpos, cpos + new Location(30), min, max))
                {
                    // TODO: Less stupid code.
                    for (int z = 0; z < 30; z++)
                    {
                        if (CollisionUtil.BoxContains(cpos + new Location(0, 0, z), cpos + new Location(30, 30, z + 1), min, max))
                        {
                            for (int x = 0; x < 30; x++)
                            {
                                if (CollisionUtil.BoxContains(cpos + new Location(x, 0, z), cpos + new Location(x + 1, 30, z + 1), min, max))
                                {
                                    for (int y = 0; y < 30; y++)
                                    {
                                        if (((Material)chunk.Value.Blocks[x, y, z].Type).OccupiesWholeBlock())
                                        {
                                            if (CollisionUtil.BoxContains(cpos + new Location(x, y, z), cpos + new Location(x + 1, y + 1, z + 1), min, max))
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
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
        public static Location BoxRayTrace(World world, Location mins, Location maxes, Location start, Location end, float bounceback = 0)
        {
            Location tend = end;
            Location normal;
            Location fnormal = Location.Zero;
            Location low = CollisionUtil.GetLow(start, end);
            Location high = CollisionUtil.GetHigh(start, end);
            foreach (KeyValuePair<Location, Chunk> chunk in world.LoadedChunks)
            {
                Location cpos = new Location(chunk.Value.X * 30, chunk.Value.Y * 30, chunk.Value.Z * 30);
                if (CollisionUtil.BoxContains(cpos, cpos + new Location(30), low + mins, high + maxes))
                {
                    // TODO: Less stupid code.
                    for (int z = 0; z < 30; z++)
                    {
                        if (CollisionUtil.BoxContains(cpos + new Location(0, 0, z), cpos + new Location(30, 30, z + 1), low + mins, high + maxes))
                        {
                            for (int x = 0; x < 30; x++)
                            {
                                if (CollisionUtil.BoxContains(cpos + new Location(x, 0, z), cpos + new Location(x + 1, 30, z + 1), low + mins, high + maxes))
                                {
                                    for (int y = 0; y < 30; y++)
                                    {
                                        if (((Material)chunk.Value.Blocks[x, y, z].Type).OccupiesWholeBlock())
                                        {
                                            Location hit = CollisionUtil.AABBClosestBox(new Location(cpos.X + x, cpos.Y + y, cpos.Z + z),
                                                Location.Zero, Location.One, mins, maxes, start, tend, out normal);
                                            if (!hit.IsNaN())
                                            {
                                                fnormal = normal;
                                                tend = hit;
                                                low = CollisionUtil.GetLow(start, tend);
                                                high = CollisionUtil.GetHigh(start, tend);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tend + bounceback * fnormal * 0.001f;
        }

    }
}
