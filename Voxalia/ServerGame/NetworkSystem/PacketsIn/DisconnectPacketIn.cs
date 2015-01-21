using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// A packet from the player, identifying that the player is leaving.
    /// </summary>
    public class DisconnectPacketIn: AbstractPacketIn
    {
        /// <summary>
        /// Constructs a received disconnect packet.
        /// </summary>
        /// <param name="sender">The player that sent this packet</param>
        public DisconnectPacketIn(Player sender, bool mode)
            : base(sender, mode)
        {
        }

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 0)
            {
                return false;
            }
            return true;
        }

        public override void Apply()
        {
            Sender.Kick("Willful disconnect");
        }
    }
}
