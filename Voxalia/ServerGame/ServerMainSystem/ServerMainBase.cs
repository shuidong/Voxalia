using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.WorldSystem;
using System.Diagnostics;
using System.Threading;
using Voxalia.ServerGame.NetworkSystem;
using Voxalia.Shared;
using Frenetic;
using Frenetic.CommandSystem;
using Voxalia.ServerGame.CommandSystem;
using Voxalia.ServerGame.PlayerCommandSystem;

namespace Voxalia.ServerGame.ServerMainSystem
{
    /// <summary>
    /// Central server entry point.
    /// </summary>
    public partial class ServerMain
    {
        /// <summary>
        /// Starts up the server.
        /// </summary>
        public static void Init()
        {
            SysConsole.Output(OutputType.INIT, "Loading server...");
            SysConsole.Output(OutputType.INIT, "Loading command engine (Frenetic)...");
            Outputter op = new ServerOutputter();
            ServerCommands.Init(op);
            ServerCVar.Init(op);
            SysConsole.Output(OutputType.INIT, "Loading console reader...");
            ConsoleHandler.Init();
            SysConsole.Output(OutputType.INIT, "Generating an empty world...");
            Worlds = new List<World>();
            // TEMPORARY
            World world = new World("world");
            Worlds.Add(world);
            SysConsole.Output(OutputType.INIT, "Loading networking engine...");
            NetworkBase.Init(true);
            SysConsole.Output(OutputType.INIT, "Loading player command engine...");
            PlayerCommandEngine.Init();
            SysConsole.Output(OutputType.INIT, "Preparing to tick...");
            // Tick
            double TARGETFPS = 20d; // TODO: CVar?
            Stopwatch Counter = new Stopwatch();
            Stopwatch DeltaCounter = new Stopwatch();
            DeltaCounter.Start();
            double TotalDelta = 0;
            double CurrentDelta = 0d;
            double TargetDelta = 0d;
            int targettime = 0;
            while (true)
            {
                // Update the tick time usage counter
                Counter.Reset();
                Counter.Start();
                // Update the tick delta counter
                DeltaCounter.Stop();
                // Delta time = Elapsed ticks * (ticks/second)
                CurrentDelta = ((double)DeltaCounter.ElapsedTicks) / ((double)Stopwatch.Frequency);
                // How much time should pass between each tick ideally
                /*TARGETFPS = ServerCVar.g_fps.ValueD;
                if (TARGETFPS < 1 || TARGETFPS > 100)
                {
                    ServerCVar.g_fps.Set("20");
                    TARGETFPS = 20;
                }*/
                TargetDelta = (1d / TARGETFPS);
                // How much delta has been built up
                TotalDelta += CurrentDelta;
                if (TotalDelta > TargetDelta * 10)
                {
                    // Lagging - cheat to catch up!
                    TargetDelta *= 3;
                }
                if (TotalDelta > TargetDelta * 10)
                {
                    // Lagging a /lot/ - cheat /extra/ to catch up!
                    TargetDelta *= 3;
                }
                if (TotalDelta > TargetDelta * 10)
                {
                    // At this point, the server's practically dead.
                    TargetDelta *= 3;
                }
                // Give up on acceleration at this point. 50 * 27 = 1.35 seconds / tick under a default tickrate.
                // As long as there's more delta built up than delta wanted, tick
                while (TotalDelta > TargetDelta)
                {
                    Tick(TargetDelta);
                    TotalDelta -= TargetDelta;
                }
                // Begin the delta counter to find out how much time is /really/ slept for
                DeltaCounter.Reset();
                DeltaCounter.Start();
                // The tick is done, stop measuring it
                Counter.Stop();
                // Only sleep for target milliseconds/tick minus how long the tick took... this is imprecise but that's okay
                targettime = (int)((1000d / TARGETFPS) - Counter.ElapsedMilliseconds);
                // Only sleep at all if we're not lagging
                if (targettime > 0)
                {
                    // Try to sleep for the target time - very imprecise, thus we deal with precision inside the tick code
                    Thread.Sleep(targettime);
                }
            }
        }
    }
}
