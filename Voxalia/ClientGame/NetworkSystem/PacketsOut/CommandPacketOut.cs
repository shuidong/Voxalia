using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.EntitySystem;
using Voxalia.Shared;

namespace Voxalia.ClientGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Sends a textual command from client to server.
    /// </summary>
    public class CommandPacketOut: AbstractPacketOut
    {
        public CommandPacketOut(string command)
        {
            ID = 3;
            Data = FileHandler.encoding.GetBytes(command);
        }
    }
}
