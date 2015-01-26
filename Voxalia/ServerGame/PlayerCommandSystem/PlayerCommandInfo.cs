using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;

namespace Voxalia.ServerGame.PlayerCommandSystem
{
    /// <summary>
    /// Holds information on an executed player command.
    /// </summary>
    public class PlayerCommandInfo
    {
        /// <summary>
        /// The player that executed the command.
        /// </summary>
        public Player Sender;

        /// <summary>
        /// The name of the executed command.
        /// </summary>
        public string CommandName;

        /// <summary>
        /// The actual executed command.
        /// </summary>
        public AbstractPlayerCommand Command; 

        /// <summary>
        /// The input arguments.
        /// </summary>
        public List<string> Arguments;
        
        /// <summary>
        /// The arguments as an imperfect space-separate string.
        /// </summary>
        public string RawArguments;
    }
}
