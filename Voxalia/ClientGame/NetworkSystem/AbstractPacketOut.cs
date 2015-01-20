using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxalia.ClientGame.NetworkSystem
{
    /// <summary>
    /// Holds information to be transmitted from the client to the server.
    /// </summary>
    public abstract class AbstractPacketOut
    {
        /// <summary>
        /// The network transmission ID of this packet type.
        /// </summary>
        public byte ID;

        /// <summary>
        /// The data held within this packet.
        /// </summary>
        public byte[] Data;
    }
}
