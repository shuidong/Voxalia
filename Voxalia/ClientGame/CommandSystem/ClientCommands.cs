﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.ClientGame.CommandSystem.CommonCommands;
using Voxalia.ClientGame.CommandSystem.NetworkCommands;
using Voxalia.ClientGame.CommandSystem.UICommands;

namespace Voxalia.ClientGame.CommandSystem
{
    /// <summary>
    /// Handles all console commands and key binds.
    /// </summary>
    class ClientCommands
    {
        /// <summary>
        /// The Commands object that all commands actually go to.
        /// </summary>
        public static Commands CommandSystem;

        /// <summary>
        /// The output system.
        /// </summary>
        public static Outputter Output;

        /// <summary>
        /// Prepares the command system, registering all base commands.
        /// </summary>
        public static void Init(Outputter _output)
        {
            // General Init
            CommandSystem = new Commands();
            Output = _output;
            CommandSystem.Output = Output;
            CommandSystem.Init();

            // UI Commands
            CommandSystem.RegisterCommand(new AttackCommand());
            CommandSystem.RegisterCommand(new BackwardCommand());
            CommandSystem.RegisterCommand(new CaptureCommand());
            CommandSystem.RegisterCommand(new DownwardCommand());
            CommandSystem.RegisterCommand(new ForwardCommand());
            CommandSystem.RegisterCommand(new ItemNextCommand());
            CommandSystem.RegisterCommand(new ItemPrevCommand());
            CommandSystem.RegisterCommand(new LeftwardCommand());
            CommandSystem.RegisterCommand(new RightwardCommand());
            CommandSystem.RegisterCommand(new SecondaryCommand());
            CommandSystem.RegisterCommand(new UpwardCommand());
            CommandSystem.RegisterCommand(new UseCommand());
            CommandSystem.RegisterCommand(new WalkCommand());

            // Common Commands
            CommandSystem.RegisterCommand(new QuitCommand());

            // Network Commands
            CommandSystem.RegisterCommand(new ConnectCommand());
        }

        /// <summary>
        /// Advances any running command queues.
        /// </summary>
        public static void Tick()
        {
            CommandSystem.Tick((float)ClientMain.Delta);
        }

        /// <summary>
        /// Executes an arbitrary list of command inputs (separated by newlines, semicolons, ...)
        /// </summary>
        /// <param name="commands">The command string to parse</param>
        public static void ExecuteCommands(string commands)
        {
            CommandSystem.ExecuteCommands(commands, null);
        }
    }
}
