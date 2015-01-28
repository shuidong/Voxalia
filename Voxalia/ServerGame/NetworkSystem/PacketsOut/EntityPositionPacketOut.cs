using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ServerGame.EntitySystem;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    public class EntityPositionPacketOut: AbstractPacketOut
    {
        public EntityPositionPacketOut(Entity e)
        {
            ID = 6;
            Data = new byte[12 + 12 + 12 + 8];
            e.Position.ToBytes().CopyTo(Data, 0);
            e.Velocity.ToBytes().CopyTo(Data, 12);
            e.Direction.ToBytes().CopyTo(Data, 12 + 12);
            BitConverter.GetBytes(e.ID).CopyTo(Data, 12 + 12 + 12);
        }
    }
}
