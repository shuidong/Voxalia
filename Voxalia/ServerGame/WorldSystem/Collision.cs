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
            Location tend = end;
            Location normal;
            Location fnormal = Location.Zero;
            foreach (KeyValuePair<Location, Chunk> chunk in world.LoadedChunks)
            {
                Location cpos = new Location(chunk.Value.X * 30, chunk.Value.Y * 30, chunk.Value.Z * 30);
                if (BoxContains(cpos, cpos + new Location(30), mins, maxes) || !AABBClosestBox(cpos, Location.Zero, new Location(30), mins, maxes, start, tend, out normal).IsNaN())
                {
                    // TODO: Less stupid code.
                    for (int z = 0; z < 30; z++)
                    {
                        Location linehit = AABBClosestBox(cpos, new Location(0, 0, z), new Location(30, 30, z + 1), mins, maxes, start, end, out normal);
                        if (!linehit.IsNaN())
                        {
                            for (int x = 0; x < 30; x++)
                            {
                                Location xhit = AABBClosestBox(cpos, new Location(x, 0, z), new Location(x + 1, 30, z + 1), mins, maxes, start, end, out normal);
                                if (!xhit.IsNaN())
                                {
                                    for (int y = 0; y < 30; y++)
                                    {
                                        if (((Material)chunk.Value.Blocks[x, y, z].Type).OccupiesWholeBlock())
                                        {
                                            Location hit = AABBClosestBox(new Location(cpos.X + x, cpos.Y + y, cpos.Z + z), Location.Zero, Location.One, mins, maxes, start, tend, out normal);
                                            if (!hit.IsNaN())
                                            {
                                                fnormal = normal;
                                                tend = hit;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return tend + (bounceback ? fnormal * 0.01f : Location.Zero);
        }

        /// <summary>
        /// Returns whether a box contains (intersects with) another box.
        /// </summary>
        /// <param name="elow">The low point for box 1</param>
        /// <param name="ehigh">The high point for box 1</param>
        /// <param name="Low">The low point for box 2</param>
        /// <param name="High">The high point for box 2</param>
        /// <returns>whether there is intersection</returns>
        public static bool BoxContains(Location elow, Location ehigh, Location Low, Location High)
        {
            return Low.X <= ehigh.X && Low.Y <= ehigh.Y && Low.Z <= ehigh.Z &&
            High.X >= elow.X && High.Y >= elow.Y && High.Z >= elow.Z;
        }

        /// <summary>
        /// Runs a collision check between two AABB objects.
        /// </summary>
        /// <param name="Position">The block's position</param>
        /// <param name="Mins">The block's mins</param>
        /// <param name="Maxs">The block's maxs</param>
        /// <param name="Mins2">The moving object's mins</param>
        /// <param name="Maxs2">The moving object's maxs</param>
        /// <param name="start">The starting location of the moving object</param>
        /// <param name="end">The ending location of the moving object</param>
        /// <param name="normal">The normal of the hit, or NaN if none</param>
        /// <returns>The location of the hit, or NaN if none</returns>
        public static Location AABBClosestBox(Location Position, Location Mins, Location Maxs, Location Mins2, Location Maxs2, Location start, Location end, out Location normal)
        {
            Location velocity = end - start;
            Location RealMins = Position + Mins;
            Location RealMaxs = Position + Maxs;
            Location RealMins2 = start + Mins2;
            Location RealMaxs2 = start + Maxs2;
            double xInvEntry, yInvEntry, zInvEntry;
            double xInvExit, yInvExit, zInvExit;
            if (end.X >= start.X)
            {
                xInvEntry = RealMins.X - RealMaxs2.X;
                xInvExit = RealMaxs.X - RealMins2.X;
            }
            else
            {
                xInvEntry = RealMaxs.X - RealMins2.X;
                xInvExit = RealMins.X - RealMaxs2.X;
            }
            if (end.Y >= start.Y)
            {
                yInvEntry = RealMins.Y - RealMaxs2.Y;
                yInvExit = RealMaxs.Y - RealMins2.Y;
            }
            else
            {
                yInvEntry = RealMaxs.Y - RealMins2.Y;
                yInvExit = RealMins.Y - RealMaxs2.Y;
            }
            if (end.Z >= start.Z)
            {
                zInvEntry = RealMins.Z - RealMaxs2.Z;
                zInvExit = RealMaxs.Z - RealMins2.Z;
            }
            else
            {
                zInvEntry = RealMaxs.Z - RealMins2.Z;
                zInvExit = RealMins.Z - RealMaxs2.Z;
            }
            double xEntry, yEntry, zEntry;
            double xExit, yExit, zExit;
            if (velocity.X == 0f)
            {
                xEntry = xInvEntry / 0.00000000000000000000000000000001f;
                xExit = xInvExit / 0.00000000000000000000000000000001f;
            }
            else
            {
                xEntry = xInvEntry / velocity.X;
                xExit = xInvExit / velocity.X;
            }
            if (velocity.Y == 0f)
            {
                yEntry = yInvEntry / 0.00000000000000000000000000000001f;
                yExit = yInvExit / 0.00000000000000000000000000000001f;
            }
            else
            {
                yEntry = yInvEntry / velocity.Y;
                yExit = yInvExit / velocity.Y;
            }
            if (velocity.Z == 0f)
            {
                zEntry = zInvEntry / 0.00000000000000000000000000000001f;
                zExit = zInvExit / 0.00000000000000000000000000000001f;
            }
            else
            {
                zEntry = zInvEntry / velocity.Z;
                zExit = zInvExit / velocity.Z;
            }
            double entryTime = Math.Max(Math.Max(xEntry, yEntry), zEntry);
            double exitTime = Math.Min(Math.Min(xExit, yExit), zExit);
            if (entryTime > exitTime || (xEntry < 0.0f && yEntry < 0.0f && zEntry < 0.0f) || xEntry > 1.0f || yEntry > 1.0f || zEntry > 1.0f)
            {
                normal = Location.NaN;
                return Location.NaN;
            }
            else
            {
                if (zEntry >= xEntry && zEntry >= yEntry)
                {
                    if (zInvEntry < 0)
                    {
                        normal = new Location(0, 0, 1);
                    }
                    else
                    {
                        normal = new Location(0, 0, -1);
                    }
                }
                else if (xEntry >= zEntry && xEntry >= yEntry)
                {
                    if (xInvEntry < 0)
                    {
                        normal = new Location(1, 0, 0);
                    }
                    else
                    {
                        normal = new Location(-1, 0, 0);
                    }
                }
                else
                {
                    if (yInvEntry < 0)
                    {
                        normal = new Location(0, 1, 0);
                    }
                    else
                    {
                        normal = new Location(0, -1, 0);
                    }
                }
                Location res = start + (end - start) * entryTime;
                return new Location(res.X, res.Y, res.Z);
            }
        }
    }
}
