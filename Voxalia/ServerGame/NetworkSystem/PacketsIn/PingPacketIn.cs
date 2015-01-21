using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.Shared;
using Voxalia.ServerGame.ServerMainSystem;

namespace Voxalia.ServerGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// A packet from the player, confirming the ping the server sent via PingPacketOut.
    /// </summary>
    public class PingPacketIn: AbstractPacketIn
    {
        /// <summary>
        /// Constructs a received ping packet.
        /// </summary>
        /// <param name="sender">The player that sent this packet</param>
        public PingPacketIn(Player sender, bool mode)
            : base(sender, mode)
        {
        }

        byte marker;

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
            if (IsChunkConnection)
            {
                if (marker != Sender.SecondayPingMarker)
                {
                    Sender.Kick("Invalid secondary ping marker, got: " + (int)marker + " while expecting " + (int)Sender.SecondayPingMarker);
                    return;
                }
                Sender.SecondayPingMarker = (byte)Utilities.UtilRandom.Next(256);
                Sender.SendToSecondary(new PingPacketOut(Sender.SecondayPingMarker));
                Sender.LastSecondaryPing = ServerMain.GlobalTickTime;
            }
            else
            {
                if (marker != Sender.PingMarker)
                {
                    Sender.Kick("Invalid ping marker, got: " + (int)marker + " while expecting " + (int)Sender.PingMarker);
                    return;
                }
                Sender.PingMarker = (byte)Utilities.UtilRandom.Next(256);
                Sender.Send(new PingPacketOut(Sender.PingMarker));
                Sender.LastPing = ServerMain.GlobalTickTime;
            }
        }
    }
}
