﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to 'use' secondarily.
    /// </summary>
    class SecondaryCommand: AbstractCommand
    {
        public SecondaryCommand()
        {
            Name = "secondary";
            Description = "Makes the player 'use' secondarily.";
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
                ClientMain.ThePlayer.Secondary = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Secondary = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Secondary = !ClientMain.ThePlayer.Secondary;
            }
        }
    }
}
