using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.EntitySystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Represents the movement of an entity.
    /// </summary>
    public class EntityPositionPacketIn: AbstractPacketIn
    {
        Location pos;

        Location vel;

        Location dir;

        ulong uid;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 12 + 12 + 12 + 8)
            {
                return false;
            }
            pos = Location.FromBytes(data, 0);
            vel = Location.FromBytes(data, 12);
            dir = Location.FromBytes(data, 12 + 12);
            uid = BitConverter.ToUInt64(data, 12 + 12 + 12);
            return true;
        }

        public override void Apply()
        {
            for (int i = 0; i < ClientMain.Entities.Count; i++)
            {
                if (ClientMain.Entities[i].ID == uid)
                {
                    ClientMain.Entities[i].Position = pos;
                    ClientMain.Entities[i].Velocity = vel;
                    ClientMain.Entities[i].Direction = dir;
                    return;
                }
            }
            SysConsole.Output(OutputType.WARNING, "Tried to move nonexistent entity with ID " + uid);
        }
    }
}
