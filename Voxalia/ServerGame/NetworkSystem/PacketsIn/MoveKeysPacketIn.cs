using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.Shared;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;

namespace Voxalia.ServerGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Receives a player's movement key and direction data.
    /// </summary>
    public class MoveKeysPacketIn: AbstractPacketIn
    {
        public MoveKeysPacketIn(Player sender, bool mode)
            : base(sender, mode)
        {
        }

        ushort MS;

        Location Direction;

        double Time;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 2 + 4 + 4 + 8)
            {
                return false;
            }
            MS = BitConverter.ToUInt16(data, 0);
            Direction.X = BitConverter.ToSingle(data, 2);
            Direction.Y = BitConverter.ToSingle(data, 2 + 4);
            Time = BitConverter.ToDouble(data, 2 + 4 + 4);
            return true;

        }

        public override void Apply()
        {
            if (Time > ServerMain.GlobalTickTime)
            {
                return; // Discard
            }
            if (Sender.LastMovePacket != null)
            {
                if (Time < Sender.LastMovePacketTime)
                {
                    return; // Discard
                }
                Sender.LastMovePacket.ApplyInternal();
                Sender.Reposition(Sender.LastMovePosition);
                Sender.Velocity = Sender.LastMoveVelocity;
                Sender.TickMovement(Time - Sender.LastMovePacketTime, true);
            }
            ApplyInternal();
            Sender.LastMovePacketTime = Time;
            Sender.LastMovePosition = Sender.Position;
            Sender.LastMoveVelocity = Sender.Velocity;
            Sender.Send(new PositionPacketOut(Sender, Time));
            Sender.LastMovePacket = this;
        }

        /// <summary>
        /// Applies the packet (for internal use).
        /// </summary>
        public void ApplyInternal()
        {
            Sender.Forward = (MS & 1) != 0;
            Sender.Backward = (MS & 2) != 0;
            Sender.Leftward = (MS & 4) != 0;
            Sender.Rightward = (MS & 8) != 0;
            Sender.Upward = (MS & 16) != 0;
            Sender.Downward = (MS & 32) != 0;
            Sender.Direction = Direction;
        }
    }
}
