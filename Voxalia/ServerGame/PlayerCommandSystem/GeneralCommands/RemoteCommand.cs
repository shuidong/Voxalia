using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.CommandSystem;

namespace Voxalia.ServerGame.PlayerCommandSystem.GeneralCommands
{
    public class RemoteCommand: AbstractPlayerCommand
    {
        public RemoteCommand()
        {
            Name = "remote";
            Description = "Executes a command as the server.";
            Usage = "";
        }

        public override void Execute(PlayerCommandInfo info)
        {
            StringBuilder rawargs = new StringBuilder();
            for (int i = 0; i < info.Arguments.Count; i++)
            {
                rawargs.Append(info.Arguments[i] + " ");
            }
            ServerCommands.CommandSystem.ExecuteCommands(rawargs.ToString(), info.Sender.Frenetic_SendMessage);
        }
    }
}
