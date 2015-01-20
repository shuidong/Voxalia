﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to move forward.
    /// </summary>
    class ForwardCommand: AbstractCommand
    {
        public ForwardCommand()
        {
            Name = "forward";
            Description = "Moves the player forward.";
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
                ClientMain.ThePlayer.Forward = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Forward = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Forward = !ClientMain.ThePlayer.Forward;
            }
        }
    }
}
