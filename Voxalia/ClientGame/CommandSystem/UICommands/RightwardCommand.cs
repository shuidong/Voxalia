﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to move rightward.
    /// </summary>
    class RightwardCommand: AbstractCommand
    {
        public RightwardCommand()
        {
            Name = "rightward";
            Description = "Moves the player rightward.";
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
                ClientMain.ThePlayer.Rightward = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Rightward = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Rightward = !ClientMain.ThePlayer.Rightward;
            }
        }
    }
}
