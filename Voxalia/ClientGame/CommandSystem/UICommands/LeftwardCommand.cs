﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to move leftward.
    /// </summary>
    class LeftwardCommand: AbstractCommand
    {
        public LeftwardCommand()
        {
            Name = "leftward";
            Description = "Moves the player leftward.";
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
                ClientMain.ThePlayer.Leftward = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Leftward = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Leftward = !ClientMain.ThePlayer.Leftward;
            }
        }
    }
}
