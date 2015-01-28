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
            if (Time > ServerMain.GlobalTickTime + 1.5)
            {
                if (ServerMain.GlobalTickTime > Sender.LastMoveWarningTime + 1)
                {
                    SysConsole.Output(OutputType.WARNING, "Client " + Sender.Username + " sent invalid move packet: time > real time");
                    Sender.LastMoveWarningTime = ServerMain.GlobalTickTime;
                }
                return; // Discard (weird ticking miscount)
            }
            if (Time > ServerMain.GlobalTickTime)
            {
                Sender.PacketsToApply.Add(this);
            }
            if (Time < ServerMain.GlobalTickTime - 1.5)
            {
                if (ServerMain.GlobalTickTime > Sender.LastMoveWarningTime + 1)
                {
                    SysConsole.Output(OutputType.WARNING, "Client " + Sender.Username + " sent invalid move packet: time < real time");
                    Sender.LastMoveWarningTime = ServerMain.GlobalTickTime;
                }
                return; // Discard (slow networking)
            }
            if (Sender.LastMovePacket != null)
            {
                if (Time < Sender.LastMovePacketTime - 0.1)
                {
                    if (ServerMain.GlobalTickTime > Sender.LastMoveWarningTime + 1)
                    {
                        SysConsole.Output(OutputType.WARNING, "Client " + Sender.Username + " sent invalid move packet: time < last time");
                        Sender.LastMoveWarningTime = ServerMain.GlobalTickTime;
                    }
                    return; // Discard (fully invalid packet, or client ticked horribly wrong!)
                }
                Sender.LastMovePacket.ApplyInternal();
                Sender.Reposition(Sender.LastMovePosition);
                Sender.Velocity = Sender.LastMoveVelocity;
                Sender.Jumped = Sender.LastJumped;
                if (Time >= Sender.LastMovePacketTime)
                {
                    Sender.TickMovement(Time - Sender.LastMovePacketTime, true);
                }
            }
            ApplyInternal();
            Sender.LastMovePacketTime = Time;
            Sender.LastMovePosition = Sender.Position;
            Sender.LastMoveVelocity = Sender.Velocity;
            Sender.LastJumped = Sender.Jumped;
            Sender.Send(new PositionPacketOut(Sender, Time));
            Sender.LastMovePacket = this;
            // TODO: is this needed?
            Sender.TickMovement(ServerMain.GlobalTickTime - Time, true);
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
            Sender.Slow = (MS & 64) != 0;
            Sender.Attack = (MS & 128) != 0;
            Sender.Secondary = (MS & 256) != 0;
            Sender.Use = (MS & 512) != 0;
            Sender.Direction = Direction;
        }
    }
}
