using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.WorldSystem;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.Shared;
using System.Threading;

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
            player.PingMarker = 0;
            player.Send(new PingPacketOut(0));
            player.SendToSecondary(new PingPacketOut(0));
            lock (Players)
            {
                Players.Add(player);
            }
            lock (Worlds[0])
            {
                Worlds[0].SpawnNewEntity(player);
                player.Reposition(new Location(1, 1, 50));
            }
        }

        /// <summary>
        /// Removes a player from the world.
        /// </summary>
        /// <param name="player">The player to remove</param>
        public static void DespawnPlayer(Player player)
        {
            lock (Players)
            {
                Players.Remove(player);
            }
            lock (player.InWorld)
            {
                player.InWorld.Remove(player);
            }
        }

        /// <summary>
        /// Creates a new world with the given name.
        /// </summary>
        /// <param name="name">The world's name</param>
        public static void CreateWorld(string name)
        {
            World world = new World(name);
            Worlds.Add(world);
            Thread thread = new Thread(new ThreadStart(world.Run));
            thread.Name = "Voxalia-TickWorld-" + world.Name;
            thread.Start();
        }
    }
}
