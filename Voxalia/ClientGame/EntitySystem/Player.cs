using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;
using Voxalia.ClientGame.ClientMainSystem;

namespace Voxalia.ClientGame.EntitySystem
{
    /// <summary>
    /// The main player (this client).
    /// </summary>
    public class Player: Entity
    {
        /// <summary>
        /// Whether the player is trying to move forward.
        /// </summary>
        public bool Forward;

        /// <summary>
        /// Whether the player is trying to move backward.
        /// </summary>
        public bool Backward;

        /// <summary>
        /// Whether the player is trying to move leftward.
        /// </summary>
        public bool Leftward;

        /// <summary>
        /// Whether the player is trying to move rightward.
        /// </summary>
        public bool Rightward;

        /// <summary>
        /// Constructs a new player, should only be done once per run.
        /// </summary>
        public Player()
            : base(true)
        {
        }

        public override void Tick()
        {
            Direction.X += MouseHandler.MouseDelta.X;
            Direction.Y += MouseHandler.MouseDelta.Y;
            while (Direction.X < 0)
            {
                Direction.X += 360;
            }
            while (Direction.X > 360)
            {
                Direction.X -= 360;
            }
            if (Direction.Y > 89.9f)
            {
                Direction.Y = 89.9f;
            }
            if (Direction.Y < -89.9f)
            {
                Direction.Y = -89.9f;
            }
            Location movement = Location.Zero;
            if (Leftward)
            {
                movement.Y = -1;
            }
            if (Rightward)
            {
                movement.Y = 1;
            }
            if (Backward)
            {
                movement.X = 1;
            }
            if (Forward)
            {
                movement.X = -1;
            }
            if (movement.LengthSquared() > 0)
            {
                movement = Utilities.RotateVector(movement, Direction.X * Utilities.PI180, Direction.Y * Utilities.PI180);
            }
            Position += movement * ClientMain.Delta;
            ClientMain.CameraEye = Position;
            ClientMain.CameraTarget = Position + Utilities.ForwardVector_Deg(Direction.X, Direction.Y);
        }
    }
}
