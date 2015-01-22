using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.WorldSystem;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.Shared;

namespace Voxalia.ServerGame.ServerMainSystem
{
    public partial class ServerMain
    {
        /// <summary>
        /// A list of all worlds the server has.
        /// </summary>
        public static List<World> Worlds;

        /// <summary>
        /// A list of all players waiting to join.
        /// </summary>
        public static List<Player> WaitingPlayers = new List<Player>();

        /// <summary>
        /// All players connected to the server.
        /// </summary>
        public static List<Player> Players = new List<Player>();

        /// <summary>
        /// Spawns a player in the world.
        /// </summary>
        /// <param name="player">The player to be spawned</param>
        public static void SpawnPlayer(Player player)
        {
            // TODO: Load player details
            Players.Add(player);
            Worlds[0].Spawn(player);
            player.PingMarker = 0;
            player.Send(new PingPacketOut(0));
            player.SendToSecondary(new PingPacketOut(0));
        }

        /// <summary>
        /// Removes a player from the world.
        /// </summary>
        /// <param name="player">The player to remove</param>
        public static void DespawnPlayer(Player player)
        {
            Players.Remove(player);
            player.InWorld.Remove(player);
        }
    }
}
