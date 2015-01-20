using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Represents an arbitrarily textual message sent to a client.
    /// </summary>
    public class MessagePacketOut: AbstractPacketOut
    {
        /// <summary>
        /// The message being sent.
        /// </summary>
        public string Message;

        public MessagePacketOut(string message)
        {
            Message = message;
            ID = 2;
            Data = Utilities.encoding.GetBytes(message);
        }
    }
}
