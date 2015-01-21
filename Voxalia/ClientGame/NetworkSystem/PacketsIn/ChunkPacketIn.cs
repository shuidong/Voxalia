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
            if (data.Length != 30 * 30 * 30 * 2 + 12)
            {
                return false;
            }
            chunkdetail = new byte[30 * 30 * 30 * 2];
            Array.Copy(data, 12, chunkdetail, 0, 30 * 30 * 30 * 2);
            loc = Location.FromBytes(data, 0);
            return true;
        }

        public override void Apply()
        {
            Chunk chunk = ClientMain.GetChunk(loc);
            chunk.FromBytes(chunkdetail);
            chunk.UpdateVBO();
        }
    }
}
