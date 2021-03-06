﻿using System;
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

        bool Jumped;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 12 + 8 + 12 + 1)
            {
                return false;
            }
            position = Location.FromBytes(data, 0);
            Time = BitConverter.ToDouble(data, 12);
            velocity = Location.FromBytes(data, 12 + 8);
            Jumped = (data[12 + 8 + 12] & 1) != 0;
            return true;
        }

        static bool flip = false;
        public override void Apply()
        {
            /*if (!ChunkPacketIn.ChunkReceived)
            {
                ClientMain.ThePlayer.Position = new Location(0, 0, 100);
                ClientMain.ThePlayer.Velocity = Location.Zero;
                return;
            }*/
            //ClientMain.SpawnEntity(new Dot() { Position = position });
            if (flip)
            {
            //    return;
            }
            //SysConsole.Output(OutputType.INFO, "Receive packet: " + position + ", " + velocity);
            
            if (Time > ClientMain.GlobalTickTime + 1.5)
            {
                SysConsole.Output(OutputType.WARNING, "Misticked heavily! ConfirmPositionPacketIn!");
                ClientMain.ThePlayer.Position = position;
                ClientMain.ThePlayer.Velocity = velocity;
                ClientMain.ThePlayer.Jumped = Jumped;
                return;
            }
            /*
            else if (Time > ClientMain.GlobalTickTime)
            {
                SysConsole.Output(OutputType.WARNING, "Misticked! ConfirmPositionPacketIn!");
                ClientMain.ThePlayer.Position = position;
                return;
            }
            */
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
                        ClientMain.ThePlayer.Position = ms.Position;
                        ClientMain.ThePlayer.Velocity = ms.Velocity;
                        ClientMain.ThePlayer.Jumped = ms.Jumped;
                        double ctime = ms.Time;
                        double Target = Time - ctime;
                        if (Target > 0)
                        {
                            ClientMain.ThePlayer.TickMovement(Target, true);
                        }
                        //SysConsole.Output(OutputType.INFO, "Move player " + (position - ClientMain.ThePlayer.Position));
                        //if (!flip)
                        {
                            //SysConsole.Output(OutputType.INFO, " Moving to position !");
                            ClientMain.ThePlayer.Position = position;
                            ClientMain.ThePlayer.Velocity = velocity;
                            ClientMain.ThePlayer.Jumped = Jumped;
                        }
                        ctime = Time;
                        for (int x = i + 1; x < ClientMain.ThePlayer.MoveStates.Count; x++)
                        {
                            ms = ClientMain.ThePlayer.MoveStates[x];
                            Target = ms.Time - ctime;
                            if (Target > 0)
                            {
                                ClientMain.ThePlayer.TickMovement(Target, true);
                            }
                            ctime = ms.Time;
                            ClientMain.ThePlayer.Forward = ms.Forward;
                            ClientMain.ThePlayer.Backward = ms.Backward;
                            ClientMain.ThePlayer.Leftward = ms.Leftward;
                            ClientMain.ThePlayer.Rightward = ms.Rightward;
                            ClientMain.ThePlayer.Upward = ms.Upward;
                            ClientMain.ThePlayer.Downward = ms.Downward;
                            ClientMain.ThePlayer.Direction = ms.Direction;
                        }
                        flip = true;
                        return;
                    }
                }
            }
            SysConsole.Output(OutputType.WARNING, "No MovementState was sufficient!");
        }
    }
}
