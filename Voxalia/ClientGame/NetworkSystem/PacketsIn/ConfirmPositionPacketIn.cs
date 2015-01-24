using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.EntitySystem;
using Voxalia.Shared;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Reads information confirming that the player's position matches the expected, or correcting the player's position.
    /// </summary>
    public class ConfirmPositionPacketIn: AbstractPacketIn
    {
        Location position;

        Location velocity;

        double Time;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 12 + 8 + 12)
            {
                return false;
            }
            position = Location.FromBytes(data, 0);
            Time = BitConverter.ToDouble(data, 12);
            velocity = Location.FromBytes(data, 12 + 8);
            return true;
        }

        public override void Apply()
        {
            if (Time > ClientMain.GlobalTickTime)
            {
                ClientMain.ThePlayer.SetPosition(position);
            }
            else
            {
                for (int i = ClientMain.ThePlayer.MoveStates.Count - 1; i >= 0; i--)
                {
                    MoveState ms = ClientMain.ThePlayer.MoveStates[i];
                    if (ms.Time < Time)
                    {
                        ClientMain.ThePlayer.AddMS();
                        ClientMain.ThePlayer.Forward = ms.Forward;
                        ClientMain.ThePlayer.Backward = ms.Backward;
                        ClientMain.ThePlayer.Leftward = ms.Leftward;
                        ClientMain.ThePlayer.Rightward = ms.Rightward;
                        ClientMain.ThePlayer.Upward = ms.Upward;
                        ClientMain.ThePlayer.Downward = ms.Downward;
                        ClientMain.ThePlayer.Direction = ms.Direction;
                        ClientMain.ThePlayer.SetPosition(position);
                        ClientMain.ThePlayer.SetVelocity(velocity);
                        double ctime = Time;
                        double Target = Time - ctime;
                        while (Target > 1d / 60d)
                        {
                            ClientMain.ThePlayer.TickMovement(1d / 60d, true);
                            Target -= 1d / 60d;
                        }
                        ClientMain.ThePlayer.TickMovement(Target, true);
                        ctime = Time;
                        for (int x = i + 1; x < ClientMain.ThePlayer.MoveStates.Count; x++)
                        {
                            ms = ClientMain.ThePlayer.MoveStates[x];
                            Target = ms.Time - ctime;
                            while (Target > 1f / 60f)
                            {
                                ClientMain.ThePlayer.TickMovement(1d / 60d, true);
                                Target -= 1d / 60d;
                            }
                            ClientMain.ThePlayer.TickMovement(Target, true);
                            ctime = ms.Time;
                            ClientMain.ThePlayer.Forward = ms.Forward;
                            ClientMain.ThePlayer.Backward = ms.Backward;
                            ClientMain.ThePlayer.Leftward = ms.Leftward;
                            ClientMain.ThePlayer.Rightward = ms.Rightward;
                            ClientMain.ThePlayer.Upward = ms.Upward;
                            ClientMain.ThePlayer.Downward = ms.Downward;
                            ClientMain.ThePlayer.Direction = ms.Direction;
                        }
                        ClientMain.ThePlayer.AddMS();
                        break;
                    }
                }
            }
        }
    }
}
