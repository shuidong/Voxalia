using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;

namespace Voxalia.ServerGame.NetworkSystem
{
    public abstract class AbstractPacketIn
    {
        /// <summary>
        /// The player that sent this packet.
        /// </summary>
        public Player Sender;

        /// <summary>
        /// Constructs an abstract packet in.
        /// </summary>
        /// <param name="sender">The player that sent this packet</param>
        public AbstractPacketIn(Player sender)
        {
            Sender = sender;
        }

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
