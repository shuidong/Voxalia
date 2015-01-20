using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    public class DisconnectPacketIn: AbstractPacketIn
    {
        string message;

        public override bool ReadBytes(byte[] data)
        {
            message = Utilities.encoding.GetString(data);
            return true;
        }

        public override void Apply()
        {
            UIConsole.WriteLine("Disconnected from server: " + message);
            ClientNetworkBase.Disconnect();
        }
    }
}
