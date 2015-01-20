using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Voxalia.ServerGame.WorldSystem
{
    /// <summary>
    /// Represents a block internally.
    /// </summary>
    public struct InternalBlockData
    {
        /// <summary>
        /// The block-type, represented by it's short ID, air by default.
        /// </summary>
        public ushort Type;
    }
}
