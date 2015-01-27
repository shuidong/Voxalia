using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Represents a single block edit.
    /// </summary>
    public class BlockPacketOut: AbstractPacketOut
    {
        /// <summary>
        /// The block location.
        /// </summary>
        Location Position;

        /// <summary>
        /// The material to change it to.
        /// </summary>
        Material Mat;

        public BlockPacketOut(Location loc, Material mat)
        {
            Position = loc;
            Mat = mat;
            ID = 4;
            Data = new byte[12 + 2];
            loc.ToBytes().CopyTo(Data, 0);
            BitConverter.GetBytes((ushort)mat).CopyTo(Data, 12);
        }
    }
}
