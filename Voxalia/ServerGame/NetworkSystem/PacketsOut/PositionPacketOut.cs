using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.ServerMainSystem;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Tells a client where they were standing at a given time.
    /// </summary>
    class PositionPacketOut: AbstractPacketOut
    {
        public PositionPacketOut(Player sender, double time)
        {
            ID = 2;
            Data = new byte[12 + 8 + 12 + 1];
            sender.Position.ToBytes().CopyTo(Data, 0);
            BitConverter.GetBytes(time).CopyTo(Data, 12);
            sender.Velocity.ToBytes().CopyTo(Data, 12 + 8);
            Data[12 + 8 + 12] = 0;
            Data[12 + 8 + 12] |= (byte)(sender.Jumped ? 1 : 0);
        }
    }
}
