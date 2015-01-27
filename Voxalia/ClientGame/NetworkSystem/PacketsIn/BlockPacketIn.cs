using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;

namespace Voxalia.ClientGame.NetworkSystem.PacketsIn
{
    /// <summary>
    /// Represents a single block change.
    /// </summary>
    public class BlockPacketIn: AbstractPacketIn
    {
        /// <summary>
        /// The location of the block to change.
        /// </summary>
        public Location Position;

        /// <summary>
        /// The new block material.
        /// </summary>
        public Material Mat;

        public override bool ReadBytes(byte[] data)
        {
            if (data.Length != 12 + 2)
            {
                return false;
            }
            Position = Location.FromBytes(data, 0);
            Mat = (Material)BitConverter.ToUInt16(data, 12);
            return true;
        }

        public override void Apply()
        {
            ClientMain.SetBlock(Position, Mat);
        }
    }
}
