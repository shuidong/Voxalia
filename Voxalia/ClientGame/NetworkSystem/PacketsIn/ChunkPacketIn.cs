using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;

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
            // TODO: Implement
            SysConsole.Output(OutputType.WARNING, "CHUNK PACKETS NOT YET IMPLEMENTED");
        }
    }
}
