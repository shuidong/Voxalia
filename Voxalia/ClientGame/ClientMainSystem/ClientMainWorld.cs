using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.WorldSystem;
using Voxalia.ClientGame.EntitySystem;
using Voxalia.Shared;

namespace Voxalia.ClientGame.ClientMainSystem
{
    public partial class ClientMain
    {
        /// <summary>
        /// A list of all loaded chunks.
        /// </summary>
        public static Dictionary<Location, Chunk> Chunks;

        /// <summary>
        /// A list of all entities loaded in the world.
        /// </summary>
        public static List<Entity> Entities;

        /// <summary>
        /// A list of all entities loaded in the world that should tick.
        /// </summary>
        public static List<Entity> Tickers;

        /// <summary>
        /// The main player (This client).
        /// </summary>
        public static Player ThePlayer;

        /// <summary>
        /// Prepares the world.
        /// </summary>
        public static void InitWorld()
        {
            ThePlayer = new Player();
            ThePlayer.Position = new Location(0, 0, 1);
            Chunks = new Dictionary<Location, Chunk>();
            Entities = new List<Entity>();
            Tickers = new List<Entity>();
            Entities.Add(ThePlayer);
            Tickers.Add(ThePlayer);
        }

        /// <summary>
        /// Ticks the actual world.
        /// </summary>
        public static void TickWorld()
        {
            for (int i = 0; i < Tickers.Count; i++)
            {
                Tickers[i].Tick();
            }
        }

        /// <summary>
        /// Gets or spawns the chunk for the given location.
        /// </summary>
        /// <param name="pos">The location</param>
        /// <returns>The chunk</returns>
        public static Chunk GetChunk(Location pos)
        {
            Chunk chunk;
            if (Chunks.TryGetValue(pos, out chunk))
            {
                return chunk;
            }
            chunk = new Chunk((int)pos.X, (int)pos.Y, (int)pos.Z);
            Chunks.Add(pos, chunk);
            return chunk;
        }
    }
}
