using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to move upward.
    /// </summary>
    class UpwardCommand: AbstractCommand
    {
        public UpwardCommand()
        {
            Name = "upward";
            Description = "Moves the player upward.";
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
                ClientMain.ThePlayer.Upward = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Upward = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Upward = !ClientMain.ThePlayer.Upward;
            }
        }
    }
}
