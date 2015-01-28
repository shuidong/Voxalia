using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;

namespace Voxalia.ServerGame.WorldSystem
{
    /// <summary>
    /// An entire in-game world.
    /// </summary>
    public class World
    {
        /// <summary>
        /// The name of the world.
        /// </summary>
        public string Name;

        /// <summary>
        /// All currently loaded chunks.
        /// </summary>
        public Dictionary<Location, Chunk> LoadedChunks;

        /// <summary>
        /// All players spawned in the world.
        /// </summary>
        public List<Player> Players;

        /// <summary>
        /// Constructs a world.
        /// </summary>
        /// <param name="name">The new name for the new world</param>
        public World(string name)
        {
            SysConsole.Output(OutputType.INIT, "Loading new world (" + name + ")...");
            LoadedChunks = new Dictionary<Location, Chunk>();
            Players = new List<Player>();
            Name = name;
        }

        /// <summary>
        /// Loads or creates a chunk at a given location.
        /// </summary>
        /// <param name="chunkLoc">The chunk coordinates</param>
        /// <returns>A chunk object</returns>
        public Chunk LoadChunk(Location chunkLoc)
        {
            Chunk chunk;
            if (LoadedChunks.TryGetValue(chunkLoc, out chunk))
            {
                return chunk;
            }
            // TEMPORARY
            chunk = new Chunk((int)chunkLoc.X, (int)chunkLoc.Y, (int)chunkLoc.Z, this);
            for (int x = 0; x < 30; x++)
            {
                for (int y = 0; y < 30; y++)
                {
                    for (int z = 0; z < 30; z++)
                    {
                        if (chunkLoc.Z == 0)
                        {
                            if (z > 15)
                            {
                                chunk.SetBlock(x, y, z, (ushort)(Utilities.UtilRandom.Next(4) + 1));
                            }
                            else
                            {
                                chunk.SetBlock(x, y, z, (ushort)1);
                            }
                        }
                        else if (chunkLoc.Z < 0)
                        {
                            chunk.SetBlock(x, y, z, (ushort)1);
                        }
                        else
                        {
                            // Nothing
                        }
                    }
                }
            }
            LoadedChunks.Add(chunkLoc, chunk);
            return chunk;
        }

        /// <summary>
        /// Tick the world (and all chunks within [and all entities within those]).
        /// </summary>
        public void Tick()
        {
            // TODO: optimize. There must be a better way to avoid errors when chunks are loaded mid-tick.
            Dictionary<Location, Chunk> chunks = new Dictionary<Location,Chunk>(LoadedChunks);
            foreach (KeyValuePair<Location, Chunk> chunk in chunks)
            {
                chunk.Value.Tick();
            }
            // TODO: Remove irrelevant chunks
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
        /// Spawns an entity in the world, placing them in the appropriate chunk.
        /// </summary>
        /// <param name="e">The entity to be spawned</param>
        public void Spawn(Entity e)
        {
            Chunk chunk = LoadChunk(GetChunkLocation(e.Position));
            chunk.Entities.Add(e);
            if (e.TickMe)
            {
                chunk.Tickers.Add(e);
            }
            e.InWorld = this;
            if (e is Player)
            {
                Players.Add((Player)e);
            }
        }

        /// <summary>
        /// Spawns an entity and broadcasts its existence to all players and assigns it a new ID.
        /// </summary>
        /// <param name="e">The entity</param>
        public void SpawnNewEntity(Entity e)
        {
            e.ID = cID++;
            Spawn(e);
            Location chloc = GetChunkLocation(e.Position);
            for (int i = 0; i < Players.Count; i++)
            {
                if (e != Players[i] && Players[i].ChunksAware.Contains(chloc))
                {
                    Players[i].Send(new NewEntityPacketOut(e));
                }
            }
        }

        /// <summary>
        /// The current unique ID the entity list is on.
        /// </summary>
        public ulong cID = 0;

        /// <summary>
        /// Despawns an entity.
        /// </summary>
        /// <param name="e">The entity to despawn</param>
        public void Remove(Entity e)
        {
            Chunk ch = LoadChunk(GetChunkLocation(e.Position));
            ch.Entities.Remove(e);
            ch.Tickers.Remove(e);
            if (e is Player)
            {
                Players.Remove((Player)e);
            }
        }

        /// <summary>
        /// Gets the block at a world location.
        /// </summary>
        /// <param name="loc">The global location</param>
        /// <returns>The block</returns>
        public Block GetBlock(Location loc)
        {
            Location ch = GetChunkLocation(loc);
            return new Block(LoadChunk(ch), (int)(Math.Floor(loc.X) - ch.X * 30), (int)(Math.Floor(loc.Y) - ch.Y * 30), (int)(Math.Floor(loc.Z) - ch.Z * 30));
        }

        /// <summary>
        /// Gets the block at a world location.
        /// </summary>
        /// <param name="loc">The global location</param>
        /// <returns>The block</returns>
        public Material GetBlockMaterial(Location loc)
        {
            Location ch = GetChunkLocation(loc);
            return (Material)LoadChunk(ch).Blocks[(int)(Math.Floor(loc.X) - ch.X * 30), (int)(Math.Floor(loc.Y) - ch.Y * 30), (int)(Math.Floor(loc.Z) - ch.Z * 30)].Type;
        }

        /// <summary>
        /// Broadcasts the details of a single block to all players on the server.
        /// </summary>
        /// <param name="loc">The block location</param>
        public void BroadcastBlock(Location loc)
        {
            loc = loc.GetBlockLocation();
            Location chunkloc = GetChunkLocation(loc);
            BlockPacketOut packet = new BlockPacketOut(loc, GetBlockMaterial(loc));
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].ChunksAware.Contains(chunkloc))
                {
                    Players[i].SendToSecondary(packet);
                }
            }
        }
    }
}
