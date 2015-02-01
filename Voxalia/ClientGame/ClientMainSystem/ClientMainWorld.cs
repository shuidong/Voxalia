using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.WorldSystem;
using Voxalia.ClientGame.EntitySystem;
using Voxalia.Shared;
using OpenTK;
using OpenTK.Graphics;
using Voxalia.ClientGame.GraphicsSystem;

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
        /// All items in the quick bar.
        /// </summary>
        public static List<Item> QuickBar = new List<Item>();

        /// <summary>
        /// The current position in the quick bar.
        /// </summary>
        public static int QuickBarPos = 0;

        /// <summary>
        /// Returns an item in the quick bar.
        /// Can return air.
        /// </summary>
        /// <param name="slot">The slot, any number is permitted</param>
        /// <returns>A valid item</returns>
        public static Item GetItemForSlot(int slot)
        {
            while (slot < 0)
            {
                slot += QuickBar.Count + 1;
            }
            while (slot > QuickBar.Count)
            {
                slot -= QuickBar.Count + 1;
            }
            if (slot == 0)
            {
                return new Item()
                {
                    Color = Color4.Transparent,
                    Image = Texture.Clear,
                    Name = "Air",
                    Description = "An empty slot.",
                    Quantity = 0
                };
            }
            else
            {
                return QuickBar[slot - 1];
            }
        }

        /// <summary>
        /// Prepares the world.
        /// </summary>
        public static void InitWorld()
        {
            ThePlayer = new Player();
            Chunks = new Dictionary<Location, Chunk>();
            Entities = new List<Entity>();
            Tickers = new List<Entity>();
            ThePlayer.ID = ulong.MaxValue;
            Entities.Add(ThePlayer);
            Tickers.Add(ThePlayer);
            QuickBar.Add(new Item()
            {
                Name = "Stone",
                Description = "A common stone block.",
                Color = Color4.White,
                Image = Texture.GetTexture("blocks/solid/stone")
            });
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
            // Remove far away chunks
            List<Location> locs = GetChunksNear(GetChunkLocation(ThePlayer.Position));
            List<Chunk> chunks = new List<Chunk>(Chunks.Values);
            for (int i = 0; i < chunks.Count; i++)
            {
                Location cur = new Location(chunks[i].X, chunks[i].Y, chunks[i].Z);
                if (!locs.Contains(cur))
                {
                    Chunks.Remove(cur);
                }
            }
        }

        static List<Location> GetChunksNear(Location pos)
        {
            List<Location> chunks = new List<Location>();
            for (int x = -3; x < 4; x++)
            {
                for (int y = -3; y < 4; y++)
                {
                    for (int z = -3; z < 4; z++)
                    {
                        chunks.Add(pos + new Location(x, y, z));
                    }
                }
            }
            return chunks;
        }

        /// <summary>
        /// Gets or spawns the chunk for the given location.
        /// </summary>
        /// <param name="pos">The chunk location</param>
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
        /// Remove an entity from the world.
        /// </summary>
        /// <param name="ent">The entity to remove</param>
        public static void RemoveEntity(Entity ent)
        {
            Entities.Remove(ent);
            if (ent.TickMe)
            {
                Tickers.Remove(ent);
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
            Location ch = GetChunkLocation(loc);
            return new Block(GetChunk(ch), (int)(Math.Floor(loc.X) - ch.X * 30), (int)(Math.Floor(loc.Y) - ch.Y * 30), (int)(Math.Floor(loc.Z) - ch.Z * 30));
        }

        /// <summary>
        /// Sets a block to a specified material.
        /// </summary>
        /// <param name="loc">The global location</param>
        /// <param name="mat">The material</param>
        public static void SetBlock(Location loc, Material mat)
        {
            loc = loc.GetBlockLocation();
            Location chunkloc = GetChunkLocation(loc);
            Chunk ch = GetChunk(chunkloc);
            ch.SetBlock((int)(Math.Floor(loc.X) - ch.X * 30), (int)(Math.Floor(loc.Y) - ch.Y * 30), (int)(Math.Floor(loc.Z) - ch.Z * 30), (ushort)mat);
            ch.UpdateVBO();
        }
    }
}
