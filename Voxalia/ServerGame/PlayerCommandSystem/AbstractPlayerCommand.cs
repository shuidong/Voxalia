using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxalia.ServerGame.PlayerCommandSystem
{
    /// <summary>
    /// Represents a command executed by a player.
    /// </summary>
    public abstract class AbstractPlayerCommand
    {
        /// <summary>
        /// The name of the command.
        /// </summary>
        public string Name;

        /// <summary>
        /// A description of the command.
        /// </summary>
        public string Description;

        /// <summary>
        /// The arguments for a command.
        /// </summary>
        public string Usage;

        /// <summary>
        /// Runs the command.
        /// </summary>
        public abstract void Execute(PlayerCommandInfo info);
    }
}
