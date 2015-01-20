using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.NetworkSystem;

namespace Voxalia.ClientGame.CommandSystem.NetworkCommands
{
    /// <summary>
    /// Allows a client to connect to a server.
    /// </summary>
    public class ConnectCommand: AbstractCommand
    {
        public ConnectCommand()
        {
            Name = "connect";
            Description = "Connects to a server (Disconnects if already connected to a server).";
            Arguments = "<host> [port]";
        }

        public override void Execute(CommandEntry entry)
        {
            if (entry.Arguments.Count < 1)
            {
                ShowUsage(entry);
                return;
            }
            string host = entry.GetArgument(0);
            string port = "28010";
            if (entry.Arguments.Count >= 2)
            {
                port = entry.GetArgument(1);
            }
            ClientNetworkBase.Connect(host, port);
        }
    }
}
