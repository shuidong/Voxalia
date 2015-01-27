using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to 'use'.
    /// </summary>
    class UseCommand: AbstractCommand
    {
        public UseCommand()
        {
            Name = "use";
            Description = "Makes the player 'use'.";
            Arguments = "";
        }

        public override void Execute(CommandEntry entry)
        {
            if (entry.Marker == 0)
            {
                entry.Bad("Must use +, -, or !");
            }
            else if (entry.Marker == 1)
            {
                ClientMain.ThePlayer.Use = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Use = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Use = !ClientMain.ThePlayer.Use;
            }
        }
    }
}
