using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.EntitySystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsOut
{
    /// <summary>
    /// Sends data showing the full exact value of the player's move key presses and facing direction.
    /// </summary>
    public class MoveKeysPacketOut: AbstractPacketOut
    {
        public MoveKeysPacketOut(MoveState ms)
        {
            ID = 2;
            Data = new byte[4 + 4 + 2 + 8];
            ushort datum = 0;
            if (ms.Forward)
            {
                datum |= 1;
            }
            if (ms.Backward)
            {
                datum |= 2;
            }
            if (ms.Leftward)
            {
                datum |= 4;
            }
            if (ms.Rightward)
            {
                datum |= 8;
            }
            if (ms.Upward)
            {
                datum |= 16;
            }
            if (ms.Downward)
            {
                datum |= 32;
            }
            if (ms.Slow)
            {
                datum |= 64;
            }
            BitConverter.GetBytes(datum).CopyTo(Data, 0);
            BitConverter.GetBytes((float)ms.Direction.X).CopyTo(Data, 2);
            BitConverter.GetBytes((float)ms.Direction.Y).CopyTo(Data, 2 + 4);
            BitConverter.GetBytes(ms.Time).CopyTo(Data, 2 + 4 + 4);
        }
    }
}
