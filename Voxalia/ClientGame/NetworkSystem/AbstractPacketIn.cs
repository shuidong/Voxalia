using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxalia.ClientGame.NetworkSystem
{
    /// <summary>
    /// Represents a packet received from the server.
    /// </summary>
    public abstract class AbstractPacketIn
    {
        /// <summary>
        /// Read the bytes sent in as a packet, and return whether it is a valid packet.
        /// </summary>
        /// <param name="data">The bytes sent in</param>
        /// <returns>Whether the packet is valid</returns>
        public abstract bool ReadBytes(byte[] data);

        /// <summary>
        /// Apply the previously read packet to the world.
        /// </summary>
        public abstract void Apply();
    }
}
