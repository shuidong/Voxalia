using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Represents a 'kick' packet: a packet telling a player that the connection will be disconnected.
    /// </summary>
    public class KickPacketOut: AbstractPacketOut
    {
        /// <summary>
        /// The kick message associated with this kick packet.
        /// </summary>
        public string Message;

        /// <summary>
        /// Constructs a kick packet out.
        /// </summary>
        /// <param name="message">The message to send to the player, explaining why they were disconnected</param>
        public KickPacketOut(string message)
        {
            Message = message;
            ID = 255;
            Data = Utilities.encoding.GetBytes(message);
        }
    }
}
