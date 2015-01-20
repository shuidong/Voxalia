using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ClientGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Represents a PING packet, used to ensure the connection is alive.
    /// </summary>
    public class PingPacketOut: AbstractPacketOut
    {
        /// <summary>
        /// Constructs a ping packet out with the given marker.
        /// </summary>
        /// <param name="marker">The marker sent by the server</param>
        public PingPacketOut(byte marker)
        {
            ID = 1;
            Data = new byte[] { marker };
        }
    }
}
