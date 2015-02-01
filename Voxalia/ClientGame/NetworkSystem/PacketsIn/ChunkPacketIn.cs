using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.ClientGame.WorldSystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    public class ChunkPacketIn: AbstractPacketIn
    {
        Location loc;

        byte[] chunkdetail;

        public override bool ReadBytes(byte[] data)
        {
            data = FileHandler.UnGZip(data);
            if (data.Length != 30 * 30 * 30 * 2 + 12)
            {
                return false;
            }
            chunkdetail = new byte[30 * 30 * 30 * 2];
            Array.Copy(data, 12, chunkdetail, 0, 30 * 30 * 30 * 2);
            loc = Location.FromBytes(data, 0);
            return true;
        }

        /// <summary>
        /// Whether a chunk has ever been received.
        /// </summary>
        public static bool ChunkReceived = false;

        /// <summary>
        /// A list of normals to update chunks on.
        /// </summary>
        public static Location[] Normals = new Location[] { new Location(-1, 0, 0),
            new Location(1, 0, 0),
            new Location(0, 1, 0),
            new Location(0, -1, 0),
            new Location(0, 0, 1),
            new Location(0, 0, -1)
        };

        public override void Apply()
        {
            Chunk chunk = ClientMain.GetChunk(loc);
            chunk.FromBytes(chunkdetail);
            chunk.UpdateVBO();
            for (int i = 0; i < Normals.Length; i++)
            {
                ClientMain.GetChunk(loc + Normals[i]).UpdateVBO();
            }
            ChunkReceived = true;
        }
    }
}
