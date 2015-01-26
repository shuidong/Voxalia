using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Represents a constantly bouncing ping signal from the server.
    /// </summary>
    public class MessagePacketIn: AbstractPacketIn
    {
        /// <summary>
        /// The message sent from the server.
        /// </summary>
        public string Message;

        public override bool ReadBytes(byte[] data)
        {
            Message = FileHandler.encoding.GetString(data);
            return true;
        }

        public override void Apply()
        {
            UIConsole.WriteLine(Message);
        }
    }
}
