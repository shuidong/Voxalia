using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxalia.ClientGame.NetworkSystem.PacketsOut
{
    class SelectionPacketOut: AbstractPacketOut
    {
        public SelectionPacketOut(int sel)
        {
            ID = 4;
            Data = BitConverter.GetBytes(sel);
        }
    }
}
