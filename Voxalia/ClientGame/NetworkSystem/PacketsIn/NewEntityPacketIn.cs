using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.GraphicsSystem;
using Voxalia.ClientGame.EntitySystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    public class NewEntityPacketIn: AbstractPacketIn
    {
        Location pos;

        Location dir;

        Location scale;

        ulong uid;

        string texture;

        string model;

        uint color;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length < 12 + 12 + 12 + 8 + 4 + 4 + 4 + 4)
            {
                return false;
            }
            pos = Location.FromBytes(data, 0);
            dir = Location.FromBytes(data, 12);
            scale = Location.FromBytes(data, 12 + 12);
            uid = BitConverter.ToUInt64(data, 12 + 12 + 12);
            int len = BitConverter.ToInt32(data, 12 + 12 + 12 + 8);
            if (data.Length < 12 + 12 + 12 + 8 + 4 + len + 4)
            {
                return false;
            }
            texture = FileHandler.encoding.GetString(data, 12 + 12 + 12 + 8 + 4, len);
            int len2 = BitConverter.ToInt32(data, 12 + 12 + 12 + 8);
            if (data.Length < 12 + 12 + 12 + 8 + 4 + len + 4 + len2 + 4)
            {
                return false;
            }
            model = FileHandler.encoding.GetString(data, 12 + 12 + 12 + 8 + 4 + len + 4, len2);
            color = BitConverter.ToUInt32(data, 12 + 12 + 12 + 8 + 4 + len + 4 + len2);
            return true;
        }

        public override void Apply()
        {
            ServerEntity ent = new ServerEntity();
            ent.Scale = scale;
            ent.Position = pos;
            ent.Direction = dir;
            ent.ID = uid;
            ent.EntTexture = Texture.GetTexture(texture);
            ent.EntModel = Model.GetModel(model);
            ent.Color = ServerEntity.IntToColor(color);
            SysConsole.Output(OutputType.INFO, "Spawn: " + ent.Position + ", " + ent.Direction + ", " + ent.Scale + ", "
                + ent.ID + ", " + ent.EntModel + ", " + ent.EntTexture + ", " + ent.Color);
            ClientMain.SpawnEntity(ent);
        }
    }
}
