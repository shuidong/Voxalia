﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.NetworkSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsIn;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.Shared;
using Voxalia.ServerGame.WorldSystem;

namespace Voxalia.ServerGame.EntitySystem
{
    public class Player: Entity
    {
        /// <summary>
        /// Whether the player is trying to move forward.
        /// </summary>
        public bool Forward = false;

        /// <summary>
        /// Whether the player is trying to move backward.
        /// </summary>
        public bool Backward = false;

        /// <summary>
        /// Whether the player is trying to move leftward.
        /// </summary>
        public bool Leftward = false;

        /// <summary>
        /// Whether the player is trying to move rightward.
        /// </summary>
        public bool Rightward = false;

        /// <summary>
        /// Whether the player is trying move upward (jump).
        /// </summary>
        public bool Upward = false;

        /// <summary>
        /// Whether the player is trying to move downward (crouch).
        /// </summary>
        public bool Downward = false;

        /// <summary>
        /// The network connection for this player.
        /// </summary>
        public Connection Network;

        /// <summary>
        /// The secondard network connection for this player.
        /// </summary>
        public Connection ChunkNetwork;

        /// <summary>
        /// This player's username.
        /// </summary>
        public string Username;

        /// <summary>
        /// The key this player used to connect.
        /// </summary>
        public string ConnectionKey;

        /// <summary>
        /// The host the player connected to, to get to the server.
        /// </summary>
        public string ConnectedHost;

        /// <summary>
        /// The port the player connected to, to get to the server.
        /// </summary>
        public string ConnectedPort;

        /// <summary>
        /// The last ping marker for this client.
        /// </summary>
        public byte PingMarker;

        /// <summary>
        /// The last ping marker on this client's secondary connection.
        /// </summary>
        public byte SecondayPingMarker;

        /// <summary>
        /// The last time the client pinged the server.
        /// </summary>
        public double LastPing;

        /// <summary>
        /// The last time the client pinged the server on the secondary connection.
        /// </summary>
        public double LastSecondaryPing;

        /// <summary>
        /// When the player first connected.
        /// </summary>
        public double JoinTime;

        /// <summary>
        /// Constructs a player entity.
        /// </summary>
        /// <param name="conn">The network connection to use</param>
        public Player(Connection conn)
            : base(true)
        {
            Network = conn;
            Position = new Location(99999f, 99999f, 99999f);
        }

        public MoveKeysPacketIn LastMovePacket = null;
        public double LastMovePacketTime = 0;
        public Location LastMovePosition = Location.Zero;

        public void TickMovement(double delta)
        {
            while (Direction.X < 0)
            {
                Direction.X += 360;
            }
            while (Direction.X > 360)
            {
                Direction.X -= 360;
            }
            if (Direction.Y > 89.9f)
            {
                Direction.Y = 89.9f;
            }
            if (Direction.Y < -89.9f)
            {
                Direction.Y = -89.9f;
            }
            Location movement = Location.Zero;
            if (Leftward)
            {
                movement.Y = -1;
            }
            if (Rightward)
            {
                movement.Y = 1;
            }
            if (Backward)
            {
                movement.X = 1;
            }
            if (Forward)
            {
                movement.X = -1;
            }
            if (movement.LengthSquared() > 0)
            {
                movement = Utilities.RotateVector(movement, Direction.X * Utilities.PI180, Direction.Y * Utilities.PI180);
            }
            Reposition(Position + movement * delta * 30);
        }

        public Chunk CurrentChunk = null;

        public override void Tick()
        {
            if (!IsValid)
            {
                return;
            }
            TickMovement(ServerMain.Delta);
            sendtimer += ServerMain.Delta;
            if (sendtimer >= 0.1f)
            {
                sendtimer = 0f;
                if (ToSend.Count > 0)
                {
                    SendToSecondary(new ChunkPacketOut(InWorld.LoadChunk(ToSend[0])));
                    ToSend.RemoveAt(0);
                }
            }
            if (Network.received > 4)
            {
                while (true)
                {
                    int len = BitConverter.ToInt32(Network.recd, 0);
                    byte type = Network.recd[4];
                    if (Network.received - 5 >= len)
                    {
                        byte[] data = new byte[len];
                        if (len > 0)
                        {
                            Array.Copy(Network.recd, 5, data, 0, len);
                        }
                        Network.received -= 5 + len;
                        byte[] newdata = new byte[Network.Max];
                        if (Network.received > 0)
                        {
                            Array.Copy(Network.recd, 5 + len, newdata, 0, Network.received);
                        }
                        Network.recd = newdata;
                        AbstractPacketIn packet;
                        switch (type)
                        {
                            case 1:
                                packet = new PingPacketIn(this, false);
                                break;
                            case 2:
                                packet = new MoveKeysPacketIn(this, false);
                                break;
                            case 255:
                                packet = new DisconnectPacketIn(this, false);
                                return;
                            default:
                                Kick("Invalid packet " + (int)type);
                                return;
                        }
                        try
                        {
                            if (packet.ReadBytes(data))
                            {
                                packet.Apply();
                            }
                            else
                            {
                                Kick("Impure packet " + (int)type);
                            }
                        }
                        catch (Exception ex)
                        {
                            SysConsole.Output(OutputType.ERROR, "Networking / player / receive packet: " + ex.ToString());
                            Kick("Invalid packet " + (int)type);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (ChunkNetwork.received > 4)
            {
                while (true)
                {
                    int len = BitConverter.ToInt32(ChunkNetwork.recd, 0);
                    byte type = ChunkNetwork.recd[4];
                    if (ChunkNetwork.received - 5 >= len)
                    {
                        byte[] data = new byte[len];
                        if (len > 0)
                        {
                            Array.Copy(ChunkNetwork.recd, 5, data, 0, len);
                        }
                        ChunkNetwork.received -= 5 + len;
                        byte[] newdata = new byte[ChunkNetwork.Max];
                        if (ChunkNetwork.received > 0)
                        {
                            Array.Copy(ChunkNetwork.recd, 5 + len, newdata, 0, ChunkNetwork.received);
                        }
                        ChunkNetwork.recd = newdata;
                        AbstractPacketIn packet;
                        switch (type)
                        {
                            case 1:
                                packet = new PingPacketIn(this, true);
                                break;
                            default:
                                Kick("Invalid packet " + (int)type);
                                return;
                        }
                        try
                        {
                            if (packet.ReadBytes(data))
                            {
                                packet.Apply();
                            }
                            else
                            {
                                Kick("Impure packet " + (int)type);
                            }
                        }
                        catch (Exception ex)
                        {
                            SysConsole.Output(OutputType.ERROR, "Networking / player / receive c-packet: " + ex.ToString());
                            Kick("Invalid packet " + (int)type);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        bool tdisco = false;

        /// <summary>
        /// Kicks the player with a given message.
        /// </summary>
        /// <param name="message">The kick message</param>
        public void Kick(string message)
        {
            tdisco = true;
            ServerMain.DespawnPlayer(this);
            Network.TickMe = false;
            ServerMain.Announce("Player " + Username + " disconnected: " + message);
            Send(new KickPacketOut(message));
            Network.InternalSocket.Close(2);
        }

        /// <summary>
        /// Send a packet along the primary socket.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void Send(AbstractPacketOut packet)
        {
            try
            {
                byte[] data = new byte[packet.Data.Length + 5];
                BitConverter.GetBytes(packet.Data.Length).CopyTo(data, 0);
                data[4] = packet.ID;
                packet.Data.CopyTo(data, 5);
                Network.InternalSocket.Send(data);
            }
            catch (Exception ex)
            {
                if (!tdisco)
                {
                    SysConsole.Output(OutputType.ERROR, "Error sending packet to player: " + ex.ToString());
                    Kick("Error sending packet");
                }
            }
        }

        /// <summary>
        /// Send a packet along the secondary socket.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void SendToSecondary(AbstractPacketOut packet)
        {
            try
            {
                byte[] data = new byte[packet.Data.Length + 5];
                BitConverter.GetBytes(packet.Data.Length).CopyTo(data, 0);
                data[4] = packet.ID;
                packet.Data.CopyTo(data, 5);
                ChunkNetwork.InternalSocket.Send(data);
            }
            catch (Exception ex)
            {
                if (!tdisco)
                {
                    SysConsole.Output(OutputType.ERROR, "Error sending packet to player (secondary): " + ex.ToString());
                    Kick("Error sending secondary packet");
                }
            }
        }

        List<Location> GetChunksNear(Location pos)
        {
            List<Location> chunks = new List<Location>();
            // TODO: Spiral algorithm?
            chunks.Add(pos + new Location(0, 0, 0));
            chunks.Add(pos + new Location(0, 0, -1));
            chunks.Add(pos + new Location(0, 0, -2));
            chunks.Add(pos + new Location(0, 0, 1));
            chunks.Add(pos + new Location(0, 0, 2));
            chunks.Add(pos + new Location(0, 1, 0));
            chunks.Add(pos + new Location(1, 0, 0));
            chunks.Add(pos + new Location(1, 1, 0));
            chunks.Add(pos + new Location(0, 1, 1));
            chunks.Add(pos + new Location(1, 0, 1));
            chunks.Add(pos + new Location(1, 1, 1));
            chunks.Add(pos + new Location(0, -1, 0));
            chunks.Add(pos + new Location(-1, 0, 0));
            chunks.Add(pos + new Location(1, 1, 0));
            chunks.Add(pos + new Location(0, 1, -1));
            chunks.Add(pos + new Location(-1, 0, -1));
            chunks.Add(pos + new Location(-1, -1, -1));
            chunks.Add(pos + new Location(0, 2, 0));
            chunks.Add(pos + new Location(0, -2, 0));
            chunks.Add(pos + new Location(-2, 0, 0));
            chunks.Add(pos + new Location(2, 0, 0));
            for (int x = -3; x < 4; x++)
            {
                for (int y = -3; y < 4; y++)
                {
                    for (int z = -3; z < 4; z++)
                    {
                        Location ch = pos + new Location(x, y, z);
                        if (!chunks.Contains(ch))
                        {
                            chunks.Add(ch);
                        }
                    }
                }
            }
            return chunks;
        }

        List<Location> ToSend = new List<Location>();

        double sendtimer = 0;

        public override void Reposition(Location pos)
        {
            Location oldpos = World.GetChunkLocation(Position);
            Location newpos = World.GetChunkLocation(pos);
            if (oldpos != newpos)
            {
                List<Location> oldchunks = GetChunksNear(oldpos);
                List<Location> newchunks = GetChunksNear(newpos);
                for (int i = 0; i < oldchunks.Count; i++)
                {
                    newchunks.Remove(oldchunks[i]);
                }
                for (int i = 0; i < newchunks.Count; i++)
                {
                    ToSend.Add(newchunks[i]);
                }
            }
            base.Reposition(pos);
        }
    }
}
