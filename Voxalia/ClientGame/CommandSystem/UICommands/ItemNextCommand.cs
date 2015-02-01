using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

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
        }
    }
}
