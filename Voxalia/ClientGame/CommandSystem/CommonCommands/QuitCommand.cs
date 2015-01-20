using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using System.Diagnostics;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.CommandSystem.CommonCommands
{
    /// <summary>
    /// A command to immediately close the client, forcefully.
    /// </summary>
    class QuitCommand: AbstractCommand
    {
        public QuitCommand()
        {
            Name = "quit";
            Description = "Quits the game, immediately.";
            Arguments = "";
        }

        public override void Execute(CommandEntry entry)
        {
            ClientMain.Window.Close();
        }
    }
}
