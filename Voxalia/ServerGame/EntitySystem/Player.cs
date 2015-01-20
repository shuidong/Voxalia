using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.NetworkSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsIn;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.Shared;

namespace Voxalia.ServerGame.EntitySystem
{
    public class Player: Entity
    {
        /// <summary>
        /// The network connection for this player.
        /// </summary>
        public Connection Network;

        /// <summary>
        /// This player's username.
        /// </summary>
        public string Username;

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
        /// Constructs a player entity.
        /// </summary>
        /// <param name="conn">The network connection to use</param>
        public Player(Connection conn)
            : base(true)
        {
            Network = conn;
        }

        public override void Tick()
        {
            if (!IsValid)
            {
                return;
            }
            if (Network.received > 4)
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
                            packet = new PingPacketIn(this);
                            break;
                        case 255:
                            packet = new DisconnectPacketIn(this);
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
            }
        }

        public void Kick(string message)
        {
            ServerMain.DespawnPlayer(this);
            Network.TickMe = false;
            ServerMain.Announce("Player " + Username + " disconnected: " + message);
            Send(new KickPacketOut(message));
            Network.InternalSocket.Close(2);
        }

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
                SysConsole.Output(OutputType.ERROR, "Error sending packet to player: " + ex.ToString());
            }
        }
    }
}
