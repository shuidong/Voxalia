﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Voxalia.Shared;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsIn;

namespace Voxalia.ServerGame.NetworkSystem
{
    /// <summary>
    /// Represents a connection to the server.
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// The internal socket this connection object wraps around.
        /// </summary>
        public Socket InternalSocket;

        /// <summary>
        /// Where in the connection process this connection is.
        /// </summary>
        public int Step = 0;

        /// <summary>
        /// Constructs a new connection.
        /// </summary>
        /// <param name="socket">The socket this connection should wrap</param>
        public Connection(Socket socket)
        {
            InternalSocket = socket;
            InternalSocket.Blocking = false;
            recd = new byte[Max];
        }

        /// <summary>
        /// Whether this connection is still alive and in need of ticking.
        /// </summary>
        public bool TickMe = true;
        
        /// <summary>
        /// How big the recd array should be.
        /// </summary>
        public int Max = 1024 * 20;

        /// <summary>
        /// Bytes currently waiting to be read.
        /// </summary>
        public byte[] recd;

        /// <summary>
        /// How many bytes are in the recd queue.
        /// </summary>
        public int received = 0;

        /// <summary>
        /// Ticks the socket.
        /// </summary>
        public void Tick()
        {
            if (!TickMe)
            {
                return;
            }
            int av = InternalSocket.Available;
            if (av > 0)
            {
                if (received + av > Max)
                {
                    InternalSocket.Close();
                    TickMe = false;
                }
                received += InternalSocket.Receive(recd, received, av, SocketFlags.None);
                if (Step == 0)
                {
                    if (received > 4)
                    {
                        if (recd[0] == 'G'
                            && recd[1] == 'E'
                            && recd[2] == 'T'
                            && recd[3] == ' ')
                        {
                            // TODO: Try for 'GET '
                            SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                + ") discarded: invalid header (GET).");
                            InternalSocket.Close();
                            TickMe = false;
                        }
                        else if (recd[0] == 'P'
                            && recd[1] == 'O'
                            && recd[2] == 'S'
                            && recd[3] == 'T'
                            && recd[4] == ' ')
                        {
                            // TODO: Try for 'POST '
                            SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                + ") discarded: invalid header (POST).");
                            InternalSocket.Close();
                            TickMe = false;
                        }
                        else if (recd[0] == 'H'
                            && recd[1] == 'E'
                            && recd[2] == 'A'
                            && recd[3] == 'D'
                            && recd[4] == ' ')
                        {
                            // TODO: Try for 'HEAD '
                            SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                + ") discarded: invalid header (HEAD).");
                            InternalSocket.Close();
                            TickMe = false;
                        }
                        else if (recd[0] == 'V'
                            && recd[1] == 'O'
                            && recd[2] == 'X'
                            && recd[3] == '_'
                            && recd[4] == ' ')
                        {
                            // Try for 'VOX_ '
                            int pos = -1;
                            for (int i = 0; i < received; i++)
                            {
                                if (recd[i] == '\n')
                                {
                                    pos = i;
                                }
                            }
                            if (pos > 0)
                            {
                                string datastr = Utilities.encoding.GetString(recd, 0, pos);
                                string[] split = datastr.Split('\r');
                                if (split.Length == 5)
                                {
                                    // VOX_ \rUsername\rEntrykey\rHost\rPort\n
                                    string username = split[1];
                                    string entrykey = split[2];
                                    string host = split[3];
                                    string port = split[4];
                                    byte[] temp = new byte[Max];
                                    Array.Copy(recd, pos + 1, temp, 0, Max - (pos + 1));
                                    received -= pos + 1;
                                    recd = temp;
                                    Step = 1;
                                    if (!Utilities.ValidateUsername(username))
                                    {
                                        SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                            + ") discarded: entirely invalid username (" + username + ") [probably an edited client].");
                                        InternalSocket.Close();
                                        TickMe = false;
                                    }
                                    else
                                    {
                                        player = new Player(this);
                                        player.Username = username;
                                        player.ConnectedHost = host;
                                        player.ConnectedPort = port;
                                        player.ConnectionKey = entrykey;
                                        player.JoinTime = ServerMain.GlobalTickTime;
                                        player.LastPing = ServerMain.GlobalTickTime;
                                        player.LastSecondaryPing = ServerMain.GlobalTickTime;
                                        InternalSocket.Send(FileHandler.encoding.GetBytes("ACCEPT\n"));
                                        // ServerMain.SpawnPlayer(player);
                                        ServerMain.WaitingPlayers.Add(player);
                                        SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                            + ") accepted: Username=" + username + ", connected to " + host + ":" + port);
                                        IsChunkNetwork = false;
                                    }
                                }
                                else if (split.Length == 2)
                                {
                                    // VOX_ \rEntrykey\n
                                    string key = split[1];
                                    byte[] temp = new byte[Max];
                                    Array.Copy(recd, pos + 1, temp, 0, Max - (pos + 1));
                                    received -= pos + 1;
                                    recd = temp;
                                    Step = 1;
                                    player = null;
                                    for (int i = 0; i < ServerMain.WaitingPlayers.Count; i++)
                                    {
                                        if (ServerMain.WaitingPlayers[i].ConnectionKey == key)
                                        {
                                            player = ServerMain.WaitingPlayers[i];
                                            break;
                                        }
                                    }
                                    if (player == null)
                                    {
                                        SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                            + ") discarded: invalid header (VOX_ secondary with unknown key).");
                                        InternalSocket.Close();
                                        TickMe = false;
                                    }
                                    else
                                    {
                                        InternalSocket.Send(FileHandler.encoding.GetBytes("ACCEPT\n"));
                                        ServerMain.WaitingPlayers.Remove(player);
                                        player.ChunkNetwork = this;
                                        ServerMain.SpawnPlayer(player);
                                        IsChunkNetwork = true;
                                        SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                            + ") accepted: Username=" + player.Username + ", now joining!");
                                    }
                                }
                                else
                                {
                                    SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                        + ") discarded: invalid header (VOX_ with invalid parameters).");
                                    InternalSocket.Close();
                                    TickMe = false;
                                }
                            }
                        }
                        else
                        {
                            SysConsole.Output(OutputType.INFO, "Connection (" + InternalSocket.RemoteEndPoint.ToString()
                                + ") discarded: invalid header (unknown).");
                            InternalSocket.Close();
                            TickMe = false;
                        }
                    }
                }
                else if (Step == 1)
                {
                    if (received > 4)
                    {
                        while (true)
                        {
                            int len = BitConverter.ToInt32(recd, 0);
                            byte type = recd[4];
                            if (received - 5 >= len)
                            {
                                byte[] data = new byte[len];
                                if (len > 0)
                                {
                                    Array.Copy(recd, 5, data, 0, len);
                                }
                                received -= 5 + len;
                                byte[] newdata = new byte[Max];
                                if (received > 0)
                                {
                                    Array.Copy(recd, 5 + len, newdata, 0, received);
                                }
                                recd = newdata;
                                AbstractPacketIn packet;
                                switch (type)
                                {
                                    case 1:
                                        packet = new PingPacketIn(player, IsChunkNetwork);
                                        break;
                                    case 2:
                                        packet = new MoveKeysPacketIn(player, IsChunkNetwork);
                                        break;
                                    case 3:
                                        packet = new CommandPacketIn(player, IsChunkNetwork);
                                        break;
                                    case 4:
                                        packet = new SelectionPacketIn(player, IsChunkNetwork);
                                        break;
                                    case 255:
                                        packet = new DisconnectPacketIn(player, IsChunkNetwork);
                                        return;
                                    default:
                                        player.Kick("Invalid packet " + (int)type);
                                        return;
                                }
                                try
                                {
                                    if (packet.ReadBytes(data))
                                    {
                                        if (packet is PingPacketIn)
                                        {
                                            packet.Apply();
                                        }
                                        else
                                        {
                                            lock (player.Packets)
                                            {
                                                player.Packets.Add(packet);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        player.Kick("Impure packet " + (int)type);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SysConsole.Output(OutputType.ERROR, "Networking / player / receive packet: " + ex.ToString());
                                    player.Kick("Invalid packet " + (int)type);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// The player this connection is for.
        /// </summary>
        Player player = null;

        /// <summary>
        /// Whether this packet came from the chunk network.
        /// </summary>
        bool IsChunkNetwork = false;
    }
}
