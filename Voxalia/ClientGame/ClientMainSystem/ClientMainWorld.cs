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

        /// <summary>
        /// Spawn an entity in the world.
        /// </summary>
        /// <param name="ent">The entity to spawn</param>
        public static void SpawnEntity(Entity ent)
        {
            Entities.Add(ent);
            if (ent.TickMe)
            {
                Tickers.Add(ent);
            }
        }

        /// <summary>
        /// Gets the chunk coordinates for the given world coordinates.
        /// </summary>
        /// <param name="worldLocation">The world coordinates</param>
        /// <returns>The chunk coordinates</returns>
        public static Location GetChunkLocation(Location worldLocation)
        {
            return new Location(Math.Floor(worldLocation.X / 30f), Math.Floor(worldLocation.Y / 30f), Math.Floor(worldLocation.Z / 30f));
        }

        /// <summary>
        /// Gets the block at a world location.
        /// </summary>
        /// <param name="loc">The global location</param>
        /// <returns>The block</returns>
        public static Block GetBlock(Location loc)
        {
            Location ch = GetChunkLocation(loc) * 30;
            return new Block(GetChunk(ch), (int)(Math.Floor(loc.X) - ch.X), (int)(Math.Floor(loc.Y) - ch.Y), (int)(Math.Floor(loc.Z) - ch.Z));
        }
    }
}
