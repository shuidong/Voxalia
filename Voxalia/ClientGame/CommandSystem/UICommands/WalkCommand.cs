using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to make the player walk.
    /// </summary>
    class WalkCommand: AbstractCommand
    {
        public WalkCommand()
        {
            Name = "walk";
            Description = "Makes the player walk.";
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
                ClientMain.ThePlayer.Slow = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Slow = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Slow = !ClientMain.ThePlayer.Slow;
            }
        }
    }
}
