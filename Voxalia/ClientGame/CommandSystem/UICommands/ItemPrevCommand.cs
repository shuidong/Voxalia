using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    public class ItemPrevCommand: AbstractCommand
    {
        public ItemPrevCommand()
        {
            Name = "itemprev";
            Description = "Selects the previous item.";
            Arguments = "";
        }

        public override void Execute(CommandEntry entry)
        {
            ClientMain.QuickBarPos--;
        }
    }
}
