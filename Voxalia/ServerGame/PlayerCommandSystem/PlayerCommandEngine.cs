using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.ServerGame.PlayerCommandSystem.GeneralCommands;

namespace Voxalia.ServerGame.PlayerCommandSystem
{
    /// <summary>
    /// Handles all client->server commands.
    /// </summary>
    public class PlayerCommandEngine
    {
        /// <summary>
        /// Prepares the player command engine.
        /// </summary>
        public static void Init()
        {
            Commands = new Dictionary<string, AbstractPlayerCommand>();
            RegisterCommand(new RemoteCommand());
        }

        /// <summary>
        /// Registers a command to the engine.
        /// </summary>
        /// <param name="cmd">The command to register</param>
        public static void RegisterCommand(AbstractPlayerCommand cmd)
        {
            Commands.Add(cmd.Name, cmd);
        }

        /// <summary>
        /// Executes a player command.
        /// </summary>
        /// <param name="sender">The player to execute the command</param>
        /// <param name="command">The command to execute</param>
        public static void Execute(Player sender, string command)
        {
            string[] split = command.Split('\n');
            PlayerCommandInfo pci = new PlayerCommandInfo();
            pci.Sender = sender;
            pci.CommandName = split[0].ToLower();
            pci.Arguments = new List<string>();
            for (int i = 1; i < split.Length; i++)
            {
                pci.Arguments.Add(split[i]);
            }
            pci.Command = GetCommand(pci.CommandName);
            StringBuilder rawargs = new StringBuilder();
            for (int i = 0; i < pci.Arguments.Count; i++)
            {
                rawargs.Append("\"" + pci.Arguments[i] + "\" ");
            }
            pci.RawArguments = rawargs.ToString();
            SysConsole.Output(OutputType.INFO, "Player " + sender.Username + " executes command: /" + pci.CommandName + " " + pci.RawArguments);
            if (pci.Command == null)
            {
                // sender.SendMessage("Unknown command :(");
                sender.SendMessage("^1Unknown command '^5" + pci.CommandName + "^r^1'.");
                SysConsole.Output(OutputType.INFO, " > Denied: Unknown");
            }
            else
            {
                if (!sender.HasPermission("commands." + pci.CommandName))
                {
                    sender.SendMessage("^1You do not have permission to use this command.");
                    SysConsole.Output(OutputType.INFO, " > Denied: No permissions");
                }
                SysConsole.Output(OutputType.INFO, " > Accepted: running");
                pci.Command.Execute(pci);
            }
        }

        static Dictionary<string, AbstractPlayerCommand> Commands;

        /// <summary>
        /// Returns the command associated with a name, or null if none.
        /// </summary>
        /// <param name="name">The command name</param>
        /// <returns>A command object</returns>
        public static AbstractPlayerCommand GetCommand(string name)
        {
            AbstractPlayerCommand cmd;
            if (Commands.TryGetValue(name, out cmd))
            {
                return cmd;
            }
            return null;
        }
    }
}
