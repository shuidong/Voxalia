﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Frenetic;
using Frenetic.CommandSystem;
using Voxalia.Shared;

namespace Voxalia.ClientGame.CommandSystem
{
    /// <summary>
    /// Handles the clientside CVar system.
    /// </summary>
    public class ClientCVar
    {
        /// <summary>
        /// The CVar System the client will use.
        /// </summary>
        public static CVarSystem system;

        // System CVars
        public static CVar s_filepath;

        // Network CVars
        public static CVar n_first;

        // Renderer CVars
        public static CVar r_fullscreen, r_width, r_height;

        // Game CVars
        public static CVar g_mapeditor;

        /// <summary>
        /// Prepares the CVar system, generating default CVars.
        /// </summary>
        public static void Init(Outputter output)
        {
            system = new CVarSystem(output);

            // System CVars
            s_filepath = Register("s_filepath", FileHandler.BaseDirectory, CVarFlag.Textual | CVarFlag.ReadOnly); // The current system environment filepath (The directory of /data).
            // Network CVars
            n_first = Register("n_first", "ipv4", CVarFlag.Textual); // Whether to prefer IPv4 or IPv6.
            // Renderer CVars
            r_fullscreen = Register("r_fullscreen", "false", CVarFlag.Boolean | CVarFlag.Delayed); // Whether to use fullscreen mode.
            r_width = Register("r_width", "800", CVarFlag.Numeric | CVarFlag.Delayed); // What width the window should be.
            r_height = Register("r_height", "600", CVarFlag.Numeric | CVarFlag.Delayed); // What height the window should be.
            // Game CVars
            g_mapeditor = Register("g_mapeditor", "false", CVarFlag.Boolean | CVarFlag.ServerControl); // Whether the client is currently in map editor mode.
        }

        static CVar Register(string name, string value, CVarFlag flags)
        {
            return system.Register(name, value, flags);
        }
    }
}
