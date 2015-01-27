using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to attack.
    /// </summary>
    class AttackCommand: AbstractCommand
    {
        public AttackCommand()
        {
            Name = "attack";
            Description = "Makes the player attack.";
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
                ClientMain.ThePlayer.Attack = true;
            }
            else if (entry.Marker == 2)
            {
                ClientMain.ThePlayer.Attack = false;
            }
            else if (entry.Marker == 3)
            {
                ClientMain.ThePlayer.Attack = !ClientMain.ThePlayer.Attack;
            }
        }
    }
}
