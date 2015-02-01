using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.Shared;

namespace Voxalia.ServerGame.NetworkSystem.PacketsIn
{
    public class SelectionPacketIn: AbstractPacketIn
    {
        public SelectionPacketIn(Player player, bool chunk)
            : base(player, chunk)
        {
        }

        int sel = 0;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 4)
            {
                return false;
            }
            sel = BitConverter.ToInt32(data, 0);
            return true;
        }

        public override void Apply()
        {
            Sender.QuickBarPos = sel;
        }
    }
}
