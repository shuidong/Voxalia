using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;
using Voxalia.ClientGame.CommandSystem;
using Voxalia.ClientGame.NetworkSystem;

namespace Voxalia.ClientGame.ClientMainSystem
{
    public partial class ClientMain
    {
        /// <summary>
        /// The time between this update tick and the last.
        /// </summary>
        public static double Delta;

        /// <summary>
        /// The primary tick entry point from the OpenGL window.
        /// </summary>
        /// <param name="sender">The sending object</param>
        /// <param name="e">The frame event details, including delta timing</param>
        static void Window_UpdateFrame(object sender, FrameEventArgs e)
        {
            try
            {
                Delta = e.Time;
                MouseHandler.Tick();
                KeyHandler.Tick();
                UIConsole.Tick();
                ClientCommands.Tick();
                ClientNetworkBase.Tick();
            }
            catch (Exception ex)
            {
                SysConsole.Output(OutputType.ERROR, "Error / updateframe: " + ex.ToString());
            }
        }
    }
}
