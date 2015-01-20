using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem
{
    /// <summary>
    /// Handles the main 'accept' portion of networking.
    /// </summary>
    public class NetworkBase
    {
        /// <summary>
        /// The primary socket that listens for new connections.
        /// </summary>
        public static Socket Listener;

        /// <summary>
        /// The primary thread that listens for new connections.
        /// </summary>
        public static Thread NetworkThread;

        /// <summary>
        /// All active connections to the server.
        /// </summary>
        public static List<Connection> Connections;

        /// <summary>
        /// Prepares the networking engine.
        /// </summary>
        /// <param name="V6">Whether to use IPv6 mode</param>
        public static void Init(bool V6)
        {
            SysConsole.Output(OutputType.INIT, "Building network socket...");
            Connections = new List<Connection>();
            Listener = new Socket(V6 ? AddressFamily.InterNetworkV6: AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            if (V6)
            {
                Listener.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, false);
            }
            SysConsole.Output(OutputType.INIT, "Binding network socket...");
            Listener.Bind(new IPEndPoint(IPAddress.IPv6Any, 28010));
            SysConsole.Output(OutputType.INIT, "Setting network socket to listen...");
            Listener.Listen(100);
            SysConsole.Output(OutputType.INIT, "Starting network thread...");
            NetworkThread = new Thread(new ThreadStart(NetworkListen));
            NetworkThread.Name = Program.GameName + "_NetworkThread";
            NetworkThread.Start();
        }

        static void NetworkListen()
        {
            while (true)
            {
                try
                {
                    Socket socket = Listener.Accept();
                    SysConsole.Output(OutputType.INFO, "Received remote connection from " + socket.RemoteEndPoint.ToString());
                    lock (Connections)
                    {
                        Connections.Add(new Connection(socket));
                    }
                }
                catch (Exception ex)
                {
                    if (ex is ThreadAbortException)
                    {
                        throw ex;
                    }
                    SysConsole.Output(OutputType.ERROR, "Error while accepting a network connection: " + ex.ToString());
                }
            }
        }

        /// <summary>
        /// Ticks the entire networking engine, receiving new packets.
        /// </summary>
        public static void Tick()
        {
            lock (Connections)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    Connections[i].Tick();
                    if (!Connections[i].TickMe)
                    {
                        Connections.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
    }
}
