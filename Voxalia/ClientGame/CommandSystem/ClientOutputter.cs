using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.UISystem;
using Frenetic;
using Voxalia.ClientGame.NetworkSystem;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;

namespace Voxalia.ClientGame.CommandSystem
{
    class ClientOutputter : Outputter
    {
        public override void WriteLine(string text)
        {
            UIConsole.WriteLine(text);
        }

        public override void Good(string tagged_text, DebugMode mode)
        {
            string text = ClientCommands.CommandSystem.TagSystem.ParseTags(tagged_text, TextStyle.Color_Outgood, null, mode);
            UIConsole.WriteLine(TextStyle.Color_Outgood + text);
        }

        public override void Bad(string tagged_text, DebugMode mode)
        {
            string text = ClientCommands.CommandSystem.TagSystem.ParseTags(tagged_text, TextStyle.Color_Outbad, null, mode);
            UIConsole.WriteLine(TextStyle.Color_Outbad + text);
        }

        public override void UnknownCommand(string basecommand, string[] arguments) // TODO: Base DebugMode?
        {
            if (ClientNetworkBase.Connected)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(basecommand);
                for (int i = 0; i < arguments.Length; i++)
                {
                    sb.Append("\n").Append(ClientCommands.CommandSystem.TagSystem.ParseTags(arguments[i], TextStyle.Color_Simple, null, DebugMode.MINIMAL));
                }
                CommandPacketOut packet = new CommandPacketOut(sb.ToString());
                ClientNetworkBase.SendPacket(packet);
            }
            else
            {
                WriteLine(TextStyle.Color_Error + "Unknown command '" +
                    TextStyle.Color_Standout + basecommand + TextStyle.Color_Error + "'.");
            }
        }
    }
}
