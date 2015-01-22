using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ServerGame.ServerMainSystem;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Represents a server to client ping packet, used to confirm a connection is alive.
    /// </summary>
    public class PingPacketOut: AbstractPacketOut
    {
        /// <summary>
        /// Constructs the ping packet out.
        /// </summary>
        /// <param name="marker">The byte ID used to confirm the response is genuine</param>
        public PingPacketOut(byte marker)
        {
            ID = 1;
            Data = new byte[9];
            BitConverter.GetBytes(ServerMain.GlobalTickTime).CopyTo(Data, 1);
            Data[0] = marker;
        }
    }
}
