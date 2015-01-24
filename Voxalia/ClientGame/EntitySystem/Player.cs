using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.Shared;
using Voxalia.ClientGame.UISystem;
using Voxalia.ClientGame.ClientMainSystem;
using Voxalia.ClientGame.NetworkSystem;
using Voxalia.ClientGame.NetworkSystem.PacketsOut;
using Voxalia.ClientGame.WorldSystem;
using BulletSharp;

namespace Voxalia.ClientGame.EntitySystem
{
    /// <summary>
    /// The main player (this client).
    /// </summary>
    public class Player: Entity
    {
        /// <summary>
        /// Half the size of the player by default.
        /// </summary>
        public static Location DefaultHalfSize = new Location(0.6f, 1.5f, 0.6f);

        /// <summary>
        /// Whether the player is trying to move forward.
        /// </summary>
        public bool Forward = false;
        bool pForward = false;

        /// <summary>
        /// Whether the player is trying to move backward.
        /// </summary>
        public bool Backward = false;
        bool pBackward = false;

        /// <summary>
        /// Whether the player is trying to move leftward.
        /// </summary>
        public bool Leftward = false;
        bool pLeftward = false;

        /// <summary>
        /// Whether the player is trying to move rightward.
        /// </summary>
        public bool Rightward = false;
        bool pRightward = false;

        /// <summary>
        /// Whether the player is trying move upward (jump).
        /// </summary>
        public bool Upward = false;
        bool pUpward = false;

        /// <summary>
        /// Whether the player is trying to move downward (crouch).
        /// </summary>
        public bool Downward = false;
        bool pDownward = false;

        Location pDirection = Location.Zero;

        /// <summary>
        /// Constructs a new player, should only be done once per run.
        /// </summary>
        public Player()
            : base(true)
        {
            Position = new Location(0, 0, 40);
        }

        /// <summary>
        /// Ticks the movement of the player.
        /// </summary>
        /// <param name="delta">How much time has passed since the last TickMovement</param>
        public void TickMovement(double delta)
        {
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
            Location target = Position + movement * delta * 30;
            if (target != Position)
            {
                // TODO: Better handling (Based on impact normal)
                Position = Collision.RayTrace(Position, new Location(target.X, Position.Y, Position.Z), true);
                Position = Collision.RayTrace(Position, new Location(Position.X, target.Y, Position.Z), true);
                Position = Collision.RayTrace(Position, new Location(Position.X, Position.Y, target.Z), true);
            }
        }

        public override void Tick()
        {
            Direction.X += MouseHandler.MouseDelta.X;
            Direction.Y += MouseHandler.MouseDelta.Y;
            TickMovement(ClientMain.Delta);
            ClientMain.CameraEye = Position + new Location(0, 0, 2.75f);
            ClientMain.CameraTarget = Position + Utilities.ForwardVector_Deg(Direction.X, Direction.Y) + new Location(0, 0, 2.75f);
            ltime += ClientMain.Delta;
            if ((Forward != pForward || Backward != pBackward
                || Leftward != pLeftward || Rightward != pRightward
                || Upward != pUpward || Downward != pDownward
                || Direction.X != pDirection.X
                || Direction.Y != pDirection.Y)
                || ltime > 0.1)
            {
                pForward = Forward;
                pBackward = Backward;
                pLeftward = Leftward;
                pRightward = Rightward;
                pUpward = Upward;
                pDownward = Downward;
                pDirection = Direction;
                ltime = 0;
                AddMS();
                if (ClientNetworkBase.Connected)
                {
                    ClientNetworkBase.SendPacket(new MoveKeysPacketOut(MoveStates[MoveStates.Count - 1]));
                }
                if (MoveStates.Count > 500)
                {
                    MoveStates.RemoveRange(0, 100);
                }
            }
        }

        double ltime = 0;

        /// <summary>
        /// All recorded movement states.
        /// </summary>
        public List<MoveState> MoveStates = new List<MoveState>();

        /// <summary>
        /// Adds a movement state to the list MoveStates.
        /// </summary>
        public void AddMS()
        {
            MoveStates.Add(new MoveState() { Position = Position, Forward = Forward,
                                             Backward = Backward,
                                             Leftward = Leftward,
                                             Rightward = Rightward,
                                             Upward = Upward,
                                             Downward = Downward,
                                             Direction = Direction,
                                             Time = ClientMain.GlobalTickTime
            });
        }

        public override void Render3D()
        {
        }

    }
    public class MoveState
    {
        public Location Position;
        public bool Forward;
        public bool Backward;
        public bool Leftward;
        public bool Rightward;
        public bool Upward;
        public bool Downward;
        public Location Direction;
        public double Time;
    }
}
