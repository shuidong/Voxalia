using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ServerGame.EntitySystem;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Despawns an entity.
    /// </summary>
    public class DespawnPacketOut: AbstractPacketOut
    {
        public DespawnPacketOut(Entity ent)
        {
            ID = 5;
            Data = BitConverter.GetBytes(ent.ID);
        }
    }
}
