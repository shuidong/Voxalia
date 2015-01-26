using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.EntitySystem;
using Voxalia.Shared;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.ServerGame.PlayerCommandSystem;

namespace Voxalia.ServerGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Receives a player's command input.
    /// </summary>
    public class CommandPacketIn: AbstractPacketIn
    {
        public CommandPacketIn(Player sender, bool mode)
            : base(sender, mode)
        {
        }

        string command;

        public override bool ReadBytes(byte[] data)
        {
            command = FileHandler.encoding.GetString(data);
            return true;
        }

        public override void Apply()
        {
            PlayerCommandEngine.Execute(Sender, command);
        }
    }
}
