using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.NetworkSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.Shared;

namespace Voxalia.ServerGame.ServerMainSystem
{
    public partial class ServerMain
    {
        /// <summary>
        /// The time, in seconds, between the last tick and this one.
        /// </summary>
        public static double Delta = 0;

        /// <summary>
        /// The current time on the server, in seconds.
        /// </summary>
        public static double GlobalTickTime = 0;

        static void OncePerSecondTick()
        {
        }

        static double secondTracker = 0;

        /// <summary>
        /// Ticks the server, include the network,
        /// and all worlds (and all chunks within those [and all entities within those]).
        /// </summary>
        /// <param name="delta">The time between the last tick and this one</param>
        public static void Tick(double delta)
        {
            Delta = delta;
            GlobalTickTime += delta;
            try
            {
                NetworkBase.Tick();
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Error / networktick: " + ex.ToString());
            }
            try
            {
                secondTracker += Delta;
                if (secondTracker >= 1.0)
                {
                    secondTracker -= 1.0;
                    OncePerSecondTick();
                }
                for (int i = 0; i < Worlds.Count; i++)
                {
                    Worlds[i].Tick();
                }
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Error / worldtick: " + ex.ToString());
            }
            try
            {
                for (int i = 0; i < WaitingPlayers.Count; i++)
                {
                    if (GlobalTickTime - WaitingPlayers[i].JoinTime > 10)
                    {
                        WaitingPlayers.RemoveAt(i);
                        i--;
                    }
                }
                for (int i = 0; i < Players.Count; i++)
                {
                    // TODO: CVar
                    if (GlobalTickTime - Players[i].LastPing > 60
                        || GlobalTickTime - Players[i].LastSecondaryPing > 60)
                    {
                        DespawnPlayer(Players[i]);
                        i--;
                    }
                }
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Error / general tick: " + ex.ToString());
            }
        }

        /// <summary>
        /// Announces (sends) a message to all clients, and the server console.
        /// </summary>
        /// <param name="message">The message to announce</param>
        public static void Announce(string message)
        {
            SysConsole.Output(OutputType.INFO, "[Announce] " + message);
            MessagePacketOut packet = new MessagePacketOut(message);
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Send(packet);
            }
        }
    }
}
