using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.ClientGame.EntitySystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Despawns an entity.
    /// </summary>
    public class DespawnEntityPacketIn: AbstractPacketIn
    {
        ulong id;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 8)
            {
                return false;
            }
            id = BitConverter.ToUInt64(data, 0);
            return true;
        }

        public override void Apply()
        {
            for (int i = 0; i < ClientMain.Entities.Count; i++)
            {
                if (ClientMain.Entities[i].ID == id)
                {
                    ClientMain.RemoveEntity(ClientMain.Entities[i]);
                    return;
                }
            }
            SysConsole.Output(OutputType.WARNING, "Tried to despawn nonexistent entity with ID " + id);
        }
    }
}
