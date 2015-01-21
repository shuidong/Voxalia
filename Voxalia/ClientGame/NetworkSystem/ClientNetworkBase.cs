using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic;
using System.Net;
using System.Net.Sockets;
using Voxalia.Shared;
using System.Threading;
using Voxalia.ClientGame.UISystem;
using Voxalia.ClientGame.NetworkSystem.PacketsIn;
using Voxalia.ClientGame.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.NetworkSystem
{
    /// <summary>
    /// Handles all clientside networking.
    /// </summary>
    public class ClientNetworkBase
    {
        /// <summary>
        /// The main socket that handles the current server connection.
        /// </summary>
        public static volatile Socket Connection;

        /// <summary>
        /// The secondary socket that handles chunk specific data.
        /// </summary>
        public static volatile Socket ChunkConnection;

        /// <summary>
        /// The thread being used to start a connection currently.
        /// </summary>
        public static volatile Thread ConnectionThread = null;

        /// <summary>
        /// Whether the client is currently connected to a server.
        /// </summary>
        public static volatile bool Connected = false;

        static byte[] buffer = new byte[1024 * 1024];
        static int bufferpos = 0;

        static byte[] buffer2 = new byte[1024 * 1024];
        static int bufferpos2 = 0;

        /// <summary>
        /// Tick the entire network engine.
        /// </summary>
        public static void Tick()
        {
            if (!Connected)
            {
                return;
            }
            try
            {
                int waiting = Connection.Available;
                if (waiting > 1024 * 1024 - bufferpos)
                {
                    waiting = 1024 * 1024 - bufferpos;
                }
                if (waiting > 0)
                {
                    Connection.Receive(buffer, bufferpos, waiting, SocketFlags.None);
                    bufferpos += waiting;
                    while (true)
                    {
                        if (bufferpos < 5)
                        {
                            break;
                        }
                        if (!Connected)
                        {
                            break;
                        }
                        int recd = BitConverter.ToInt32(buffer, 0);
                        if (bufferpos < 5 + recd)
                        {
                            break;
                        }
                        byte[] packet = new byte[recd];
                        Array.Copy(buffer, 5, packet, 0, recd);
                        int ID = buffer[4];
                        Array.Copy(buffer, recd + 5, buffer, 0, bufferpos - (recd + 5));
                        bufferpos -= recd + 5;
                        AbstractPacketIn apacket = null;
                        switch (ID)
                        {
                            case 1:
                                apacket = new PingPacketIn();
                                break;
                            case 255:
                                apacket = new DisconnectPacketIn();
                                break;
                            default:
                                UIConsole.WriteLine("Invalid packet from server, ID " + ID);
                                break;
                        }
                        if (apacket != null)
                        {
                            ReceivePacket(ID, apacket, packet, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Networking: " + ex.ToString());
                Disconnect();
            }
            try
            {
                int waiting = ChunkConnection.Available;
                if (waiting > 1024 * 1024 - bufferpos2)
                {
                    waiting = 1024 * 1024 - bufferpos2;
                }
                if (waiting > 0)
                {
                    ChunkConnection.Receive(buffer2, bufferpos2, waiting, SocketFlags.None);
                    bufferpos2 += waiting;
                    while (true)
                    {
                        if (bufferpos2 < 5)
                        {
                            break;
                        }
                        if (!Connected)
                        {
                            break;
                        }
                        int recd = BitConverter.ToInt32(buffer2, 0);
                        if (bufferpos2 < 5 + recd)
                        {
                            break;
                        }
                        byte[] packet = new byte[recd];
                        Array.Copy(buffer2, 5, packet, 0, recd);
                        int ID = buffer2[4];
                        Array.Copy(buffer2, recd + 5, buffer2, 0, bufferpos2 - (recd + 5));
                        bufferpos2 -= recd + 5;
                        AbstractPacketIn apacket = null;
                        switch (ID)
                        {
                            case 0:
                                apacket = new ChunkPacketIn();
                                break;
                            case 1:
                                apacket = new PingPacketIn();
                                break;
                            default:
                                UIConsole.WriteLine("Invalid secondary packet from server, ID " + ID);
                                break;
                        }
                        if (apacket != null)
                        {
                            ReceivePacket(ID, apacket, packet, true);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Networking (Secondary): " + ex.ToString());
                Disconnect();
            }
        }

        /// <summary>
        /// Receive a packet and apply it.
        /// </summary>
        /// <param name="ID">The packet type ID</param>
        /// <param name="packet">The packet object itself</param>
        /// <param name="data">The data in the packet</param>
        public static void ReceivePacket(int ID, AbstractPacketIn packet, byte[] data, bool IsChunkPacket)
        {
            try
            {
                packet.IsChunkConnection = IsChunkPacket;
                if (packet.ReadBytes(data))
                {
                    packet.Apply();
                }
                else
                {
                    UIConsole.WriteLine(TextStyle.Color_Error + "Impure packet from server, ID " + ID + " ICP:" + IsChunkPacket);
                }
            }
            catch (Exception ex)
            {
                UIConsole.WriteLine(TextStyle.Color_Error + "Bad packet from server, ID " + ID + " ICP:" + IsChunkPacket + ": " + ex.ToString());
            }
        }

        /// <summary>
        /// Sends a packet from the client to the server.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public static void SendPacket(AbstractPacketOut packet)
        {
            if (!Connected)
            {
                throw new Exception("Tried to send packet while not ready to send packets!");
            }
            try
            {
                byte[] data = packet.Data;
                byte[] full = new byte[data.Length + 5];
                BitConverter.GetBytes(data.Length).CopyTo(full, 0);
                full[4] = (byte)packet.ID;
                data.CopyTo(full, 5);
                Connection.Send(full);
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Networking / send packet (" + packet.ID + "): " + ex.ToString());
                Disconnect();
            }
        }

        /// <summary>
        /// Sends a packet from the client to the server down the secondary packet channel.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public static void SendPacketToSecondary(AbstractPacketOut packet)
        {
            if (!Connected)
            {
                throw new Exception("Tried to send packet while not ready to send packets!");
            }
            try
            {
                byte[] data = packet.Data;
                byte[] full = new byte[data.Length + 5];
                BitConverter.GetBytes(data.Length).CopyTo(full, 0);
                full[4] = (byte)packet.ID;
                data.CopyTo(full, 5);
                ChunkConnection.Send(full);
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Networking / send packet [secondary] (" + packet.ID + "): " + ex.ToString());
                Disconnect();
            }
        }

        /// <summary>
        /// Connects to a server.
        /// </summary>
        /// <param name="host">The hostname to connect to</param>
        /// <param name="port">The port on the host to connect to</param>
        public static void Connect(string host, string port)
        {
            Disconnect();
            CurrentHost = host;
            CurrentPort = Utilities.StringToUShort(port);
            ConnectionThread = new Thread(new ThreadStart(ConnectInternal));
            UIConsole.WriteLine("^r^7Connected to " + host + "^r^7:" + port);
            ConnectionThread.Start();
        }

        /// <summary>
        /// Disconnects any active connection.
        /// </summary>
        public static void Disconnect()
        {
            bool HadToDisco = false;
            try
            {
                if (ConnectionThread != null && ConnectionThread.IsAlive)
                {
                    HadToDisco = true;
                    ConnectionThread.Abort();
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException)
                {
                    throw ex;
                }
                SysConsole.Output(OutputType.ERROR, "Network / Disconnect / ThreadClose: " + ex.ToString());
            }
            try
            {
                if (Connection != null)
                {
                    if (Connection.Connected && Connected)
                    {
                        // TODO: Send disconnect packet within a try/catch.
                    }
                    HadToDisco = true;
                    Connection.Close(5);
                }
                if (ChunkConnection != null)
                {
                    HadToDisco = true;
                    Connection.Close(5);
                }
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException)
                {
                    throw ex;
                }
                SysConsole.Output(OutputType.ERROR, "Network / Disconnect / SocketClose: " + ex.ToString());
            }
            ConnectionThread = null;
            Connection = null;
            Connected = false;
            if (HadToDisco)
            {
                UIConsole.WriteLine("^3Disconnected from server.");
            }
        }

        static void ConnectInternal()
        {
            try
            {
                string key = Utilities.UtilRandom.NextDouble().ToString();
                IPAddress address;
                if (!IPAddress.TryParse(CurrentHost, out address))
                {
                    IPHostEntry entry = Dns.GetHostEntry(CurrentHost);
                    if (entry.AddressList.Length == 0)
                    {
                        throw new Exception("Empty address list for DNS server at '" + CurrentHost + "'");
                    }
                    if (ClientCVar.n_first.Value.ToLower() == "ipv4")
                    {
                        foreach (IPAddress saddress in entry.AddressList)
                        {
                            if (saddress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                address = saddress;
                                break;
                            }
                        }
                    }
                    else if (ClientCVar.n_first.Value.ToLower() == "ipv6")
                    {
                        foreach (IPAddress saddress in entry.AddressList)
                        {
                            if (saddress.AddressFamily == AddressFamily.InterNetworkV6)
                            {
                                address = saddress;
                                break;
                            }
                        }
                    }
                    if (address == null)
                    {
                        foreach (IPAddress saddress in entry.AddressList)
                        {
                            if (saddress.AddressFamily == AddressFamily.InterNetworkV6 || saddress.AddressFamily == AddressFamily.InterNetwork)
                            {
                                address = saddress;
                                break;
                            }
                        }
                    }
                    if (address == null)
                    {
                        throw new Exception("DNS has entries, but none are IPv4 or IPv6!");
                    }
                }
                Connection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Connection.LingerState.LingerTime = 5;
                Connection.LingerState.Enabled = true;
                Connection.ReceiveTimeout = 10000;
                Connection.SendTimeout = 10000;
                Connection.ReceiveBufferSize = 5 * 1024 * 1024;
                Connection.SendBufferSize = 5 * 1024 * 1024;
                Connection.Connect(new IPEndPoint(address, CurrentPort));
                Connection.Send(FileHandler.encoding.GetBytes("VOX_ \r" + ClientMain.Username
                    + "\r" + key + "\r" + CurrentHost + "\r" + CurrentPort + "\n"));
                byte[] resp = ReceiveUntil(Connection, 50, (byte)'\n');
                if (FileHandler.encoding.GetString(resp) != "ACCEPT")
                {
                    Connection.Close();
                    throw new Exception("Server did not accept connection");
                }
                // Now, establish chunk connection
                ChunkConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                ChunkConnection.LingerState.LingerTime = 5;
                ChunkConnection.LingerState.Enabled = true;
                ChunkConnection.ReceiveTimeout = 10000;
                ChunkConnection.SendTimeout = 10000;
                ChunkConnection.ReceiveBufferSize = 5 * 1024 * 1024;
                ChunkConnection.SendBufferSize = 5 * 1024 * 1024;
                ChunkConnection.Connect(new IPEndPoint(address, CurrentPort));
                ChunkConnection.Send(FileHandler.encoding.GetBytes("VOX_ \r" + key + "\n"));
                resp = ReceiveUntil(ChunkConnection, 50, (byte)'\n');
                if (FileHandler.encoding.GetString(resp) != "ACCEPT")
                {
                    Connection.Close();
                    ChunkConnection.Close();
                    throw new Exception("Server did not accept connection");
                }
                SysConsole.Output(OutputType.INFO, "Connected to " + address.ToString() + ":" + CurrentPort);
                Connected = true;
            }
            catch (Exception ex)
            {
                if (ex is ThreadAbortException)
                {
                    throw ex;
                }
                SysConsole.Output(OutputType.ERROR, "Networking / connect internal: " + ex.ToString());
                // TODO: Schedule disconnect
            }
        }

        static byte[] ReceiveFully(Socket s, int bytecount)
        {
            byte[] bytes = new byte[bytecount];
            int gotten = 0;
            while (gotten < bytecount)
            {
                gotten += s.Receive(bytes, gotten, bytecount - gotten, SocketFlags.None);
            }
            return bytes;
        }

        static byte[] ReceiveUntil(Socket s, int max_bytecount, byte ender)
        {
            byte[] bytes = new byte[max_bytecount];
            int gotten = 0;
            while (gotten < max_bytecount)
            {
                s.Receive(bytes, gotten, 1, SocketFlags.None);
                if (bytes[gotten] == ender)
                {
                    byte[] got = new byte[gotten];
                    Array.Copy(bytes, got, gotten);
                    return got;
                }
                gotten++;
            }
            throw new Exception("Maximum byte count reached without valid ender");
        }

        /// <summary>
        /// The hostname currently connected to.
        /// </summary>
        public static volatile string CurrentHost;

        /// <summary>
        /// The port on the host currently connected to.
        /// </summary>
        public static volatile ushort CurrentPort;
    }
}
