using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to move downward.
    /// </summary>
    class DownwardCommand: AbstractCommand
    {
        public DownwardCommand()
        {
            Name = "downward";
            Description = "Moves the player downward.";
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
                ClientMain.ThePlayer.Downward = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Downward = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Downward = !ClientMain.ThePlayer.Downward;
            }
        }
    }
}
