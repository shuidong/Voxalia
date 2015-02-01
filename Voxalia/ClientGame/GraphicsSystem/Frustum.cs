using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Voxalia.Shared;

namespace Voxalia.ClientGame.GraphicsSystem
{
    /// <summary>
    /// Represents a 3D Frustum.
    /// </summary>
    public class Frustum
    {
        Plane Near;

        Plane Far;

        Plane Left;

        Plane Right;

        Plane Top;

        Plane Bottom;

        public Frustum(Matrix4 matrix)
        {
            Left = new Plane(new Location(-matrix.M14 - matrix.M11, -matrix.M24 - matrix.M21, -matrix.M34 - matrix.M31), -matrix.M44 - matrix.M41);
            Right = new Plane(new Location(matrix.M11 - matrix.M14, matrix.M21 - matrix.M24, matrix.M31 - matrix.M34), matrix.M41 - matrix.M44);
            Top = new Plane(new Location(matrix.M12 - matrix.M14, matrix.M22 - matrix.M24, matrix.M32 - matrix.M34), matrix.M42 - matrix.M44);
            Bottom = new Plane(new Location(-matrix.M14 - matrix.M12, -matrix.M24 - matrix.M22, -matrix.M34 - matrix.M32), -matrix.M44 - matrix.M42);
            Near = new Plane(new Location(-matrix.M13, -matrix.M23, -matrix.M33), -matrix.M43);
            Far = new Plane(new Location(matrix.M13 - matrix.M14, matrix.M23 - matrix.M24, matrix.M33 - matrix.M34), matrix.M43 - matrix.M44);
        }

        /// <summary>
        /// Returns whether an AABB is contained by the Frustum.
        /// </summary>
        /// <param name="min">The lower coord of the AABB</param>
        /// <param name="max">The higher coord of the AABB</param>
        /// <returns>Whether it is contained</returns>
        public bool ContainsBox(Location min, Location max)
        {
            if (min == max)
            {
                return Contains(min);
            }
            bool any = false;
            if (Contains(min)) { any = true; }
            else if (Contains(max)) { any = true; }
            else if (Contains(new Location(min.X, min.Y, max.Z))) { any = true; }
            else if (Contains(new Location(min.X, max.Y, max.Z))) { any = true; }
            else if (Contains(new Location(max.X, min.Y, max.Z))) { any = true; }
            else if (Contains(new Location(max.X, min.Y, min.Z))) { any = true; }
            else if (Contains(new Location(max.X, max.Y, min.Z))) { any = true; }
            else if (Contains(new Location(min.X, max.Y, min.Z))) { any = true; }
            return any;
        }

        /// <summary>
        /// Returns whether the Frustum contains a point
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns>Whether it's contained</returns>
        public bool Contains(Location point)
        {
            double rel = TryPoint(point, Far);
            if (rel > 0) { return false; }
            rel = TryPoint(point, Near);
            if (rel > 0) { return false; }
            rel = TryPoint(point, Top);
            if (rel > 0) { return false; }
            rel = TryPoint(point, Bottom);
            if (rel > 0) { return false; }
            rel = TryPoint(point, Left);
            if (rel > 0) { return false; }
            rel = TryPoint(point, Right);
            if (rel > 0) { return false; }
            return true;
        }

        double TryPoint(Location point, Plane plane)
        {
            return point.X * plane.Normal.X + point.Y * plane.Normal.Y + point.Z * plane.Normal.Z + plane.D;
        }
    }
}
