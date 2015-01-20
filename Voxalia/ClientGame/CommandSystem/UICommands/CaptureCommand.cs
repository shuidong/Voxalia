using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic.CommandSystem;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.CommandSystem.UICommands
{
    /// <summary>
    /// A command to capture the mouse.
    /// </summary>
    public class CaptureCommand: AbstractCommand
    {
        public CaptureCommand()
        {
            Name = "capture";
            Description = "Captures or releases the mouse.";
            Arguments = "";
        }

        public override void Execute(CommandEntry entry)
        {
            if (MouseHandler.MouseCaptured)
            {
                entry.Good("Mouse released.");
                MouseHandler.ReleaseMouse();
            }
            else
            {
                entry.Good("Mouse captured.");
                MouseHandler.CaptureMouse();
            }
        }
    }
}
