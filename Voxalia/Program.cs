using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if CLIENT
using Voxalia.ClientGame.ClientMainSystem;
#endif
using Voxalia.ServerGame.ServerMainSystem;
using System.Diagnostics;
using Voxalia.Shared;

namespace Voxalia
{
    /// <summary>
    /// Central program entry point.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The name of the game.
        /// </summary>
        public static string GameName = "Voxalia";

        public static IntPtr ConsoleHandle;

        /// <summary>
        /// Central program entry point.
        /// Decides whether to lauch the server or the client.
        /// </summary>
        /// <param name="args">The command line arguments</param>
        static void Main(string[] args)
        {
            ConsoleHandle = Process.GetCurrentProcess().MainWindowHandle;
            SysConsole.Init();
#if CLIENT
            if (args.Length > 0 && args[0] == "server")
            {
                ServerMain.Init();
            }
            else
            {
                ClientMain.Init();
            }
#else
            ServerMain.Init();
#endif
        }
    }
}
