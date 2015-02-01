using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.ClientGame.NetworkSystem;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    public class ItemNextCommand: AbstractCommand
    {
        public ItemNextCommand()
        {
            Name = "itemnext";
            Description = "Selects the next item.";
            Arguments = "";
        }

        public override void Execute(CommandEntry entry)
        {
            ClientMain.QuickBarPos++;
            if (ClientMain.QuickBarPos > ClientMain.QuickBar.Count)
            {
                ClientMain.QuickBarPos -= ClientMain.QuickBar.Count + 1;
            }
            if (ClientNetworkBase.Connected)
            {
                ClientNetworkBase.SendPacket(new SelectionPacketOut(ClientMain.QuickBarPos));
            }
        }
    }
}
