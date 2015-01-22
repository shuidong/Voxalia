using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Represents a constantly bouncing ping signal from the server.
    /// </summary>
    public class PingPacketIn: AbstractPacketIn
    {
        /// <summary>
        /// The byte chosen at random by the server to ensure ping packets are genuine.
        /// </summary>
        public byte marker;

        /// <summary>
        /// The time the server is currently at.
        /// </summary>
        public double Time;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 9)
            {
                return false;
            }
            marker = data[0];
            Time = BitConverter.ToDouble(data, 1);
            return true;
        }

        public override void Apply()
        {
            if (IsChunkConnection)
            {
                ClientNetworkBase.SendPacketToSecondary(new PingPacketOut(marker));
            }
            else
            {
                ClientNetworkBase.SendPacket(new PingPacketOut(marker));
                if (Math.Abs(ClientMain.GlobalTickTime - Time) > 1)
                {
                    ClientMain.GlobalTickTime = Time;
                }
                else if (ClientMain.GlobalTickTime - Time > 0.1)
                {
                    ClientMain.GlobalTickTime -= 0.1;
                }
                else if (ClientMain.GlobalTickTime - Time < -0.1)
                {
                    ClientMain.GlobalTickTime += 0.1;
                }
                else if (ClientMain.GlobalTickTime - Time > 0.01)
                {
                    ClientMain.GlobalTickTime -= 0.01;
                }
                else if (ClientMain.GlobalTickTime - Time < -0.01)
                {
                    ClientMain.GlobalTickTime += 0.01;
                }
            }
        }
    }
}
