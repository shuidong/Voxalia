using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem.PacketsOut
{
    public class NewEntityPacketOut: AbstractPacketOut
    {
        Location pos;

        Location dir;

        Location scale;

        ulong uid;

        string texture;

        string model;

        uint color;

        public NewEntityPacketOut(Entity entity)
        {
            pos = entity.Position;
            dir = entity.Direction;
            scale = entity.Scale;
            uid = entity.ID;
            texture = entity.Texture;
            model = entity.Model;
            color = entity.Color;
            ID = 4;
            GenerateData();
        }

        /// <summary>
        /// Generates the Data of this packet from the current field values.
        /// </summary>
        public void GenerateData()
        {
            byte[] texbytes = FileHandler.encoding.GetBytes(texture);
            byte[] modbytes = FileHandler.encoding.GetBytes(model);
            Data = new byte[12 + 12 + 12 + 8 + 4 + texbytes.Length + 4 + modbytes.Length + 4];
            pos.ToBytes().CopyTo(Data, 0);
            dir.ToBytes().CopyTo(Data, 12);
            scale.ToBytes().CopyTo(Data, 12 + 12);
            BitConverter.GetBytes(uid).CopyTo(Data, 12 + 12 + 12);
            BitConverter.GetBytes(texbytes.Length).CopyTo(Data, 12 + 12 + 12 + 8);
            texbytes.CopyTo(Data, 12 + 12 + 12 + 8 + 4);
            BitConverter.GetBytes(texbytes.Length).CopyTo(Data, 12 + 12 + 12 + 8 + 4 + texbytes.Length);
            texbytes.CopyTo(Data, 12 + 12 + 12 + 8 + 4 + texbytes.Length + 4);
            BitConverter.GetBytes(color).CopyTo(Data, 12 + 12 + 12 + 8 + 4 + texbytes.Length + 4 + modbytes.Length);
        }
    }
}
