using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    public class PingPacketIn: AbstractPacketIn
    {
        /// <summary>
        /// The byte chosen at random by the server to ensure ping packets are genuine.
        /// </summary>
        public byte marker;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 1)
            {
                return false;
            }
            marker = data[0];
            return true;
        }

        public override void Apply()
        {
            ClientNetworkBase.SendPacket(new PingPacketOut(marker));
        }
    }
}
