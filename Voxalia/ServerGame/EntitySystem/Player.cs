using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Voxalia.ServerGame.NetworkSystem;
using Voxalia.ServerGame.NetworkSystem.PacketsIn;
using Voxalia.ServerGame.NetworkSystem.PacketsOut;
using Voxalia.ServerGame.ServerMainSystem;
using Voxalia.ServerGame.CommandSystem;
using Voxalia.Shared;
using Voxalia.ServerGame.WorldSystem;
using Frenetic.CommandSystem;
using Frenetic;

namespace Voxalia.ServerGame.EntitySystem
{
    public class Player: Entity
    {
        /// <summary>
        /// Half the size of the player by default.
        /// </summary>
        public static Location DefaultHalfSize = new Location(0.6f, 0.6f, 1.5f);

        /// <summary>
        /// Whether the player is trying to move forward.
        /// </summary>
        public bool Forward = false;

        /// <summary>
        /// Whether the player is trying to move backward.
        /// </summary>
        public bool Backward = false;

        /// <summary>
        /// Whether the player is trying to move leftward.
        /// </summary>
        public bool Leftward = false;

        /// <summary>
        /// Whether the player is trying to move rightward.
        /// </summary>
        public bool Rightward = false;

        /// <summary>
        /// Whether the player has jumped since the last time they pressed space.
        /// </summary>
        public bool Jumped = false;

        /// <summary>
        /// Whether the player is trying move upward (jump).
        /// </summary>
        public bool Upward = false;

        /// <summary>
        /// Whether the player is trying to move downward (crouch).
        /// </summary>
        public bool Downward = false;

        /// <summary>
        /// Whether the player is moving slowly (walking).
        /// </summary>
        public bool Slow = false;

        /// <summary>
        /// Whether the player is attacking.
        /// </summary>
        public bool Attack = false;

        /// <summary>
        /// Whether the player is 'using' secondarily.
        /// </summary>
        public bool Secondary = false;

        /// <summary>
        /// Whether the player is 'using'.
        /// </summary>
        public bool Use = false;

        /// <summary>
        /// The network connection for this player.
        /// </summary>
        public Connection Network;

        /// <summary>
        /// The secondard network connection for this player.
        /// </summary>
        public Connection ChunkNetwork;

        /// <summary>
        /// This player's username.
        /// </summary>
        public string Username;

        /// <summary>
        /// The key this player used to connect.
        /// </summary>
        public string ConnectionKey;

        /// <summary>
        /// The host the player connected to, to get to the server.
        /// </summary>
        public string ConnectedHost;

        /// <summary>
        /// The port the player connected to, to get to the server.
        /// </summary>
        public string ConnectedPort;

        /// <summary>
        /// The last ping marker for this client.
        /// </summary>
        public byte PingMarker;

        /// <summary>
        /// The last ping marker on this client's secondary connection.
        /// </summary>
        public byte SecondayPingMarker;

        /// <summary>
        /// The last time the client pinged the server.
        /// </summary>
        public double LastPing;

        /// <summary>
        /// The last time the client pinged the server on the secondary connection.
        /// </summary>
        public double LastSecondaryPing;

        /// <summary>
        /// When the player first connected.
        /// </summary>
        public double JoinTime;

        /// <summary>
        /// When was the last time the MoveKeysPacketIn showed a warning for this client (if ever).
        /// </summary>
        public double LastMoveWarningTime = 0;

        /// <summary>
        /// Constructs a player entity.
        /// </summary>
        /// <param name="conn">The network connection to use</param>
        public Player(Connection conn)
            : base(true)
        {
            Network = conn;
            Position = new Location(99999f, 99999f, 99999f);
            Scale = new Location(10);
        }

        /// <summary>
        /// Returns whether a client has the given permission value.
        /// </summary>
        /// <param name="permission">The permission key</param>
        /// <returns>Whether the player has it</returns>
        public bool HasPermission(string permission)
        {
            return true;//TODO
        }

        public MoveKeysPacketIn LastMovePacket = null;
        public double LastMovePacketTime = 0;
        public Location LastMovePosition = Location.Zero;
        public Location LastMoveVelocity = Location.Zero;
        public bool LastJumped = false;

        public Location Maxes = DefaultHalfSize;


        /// <summary>
        /// Used by the /remote command.
        /// </summary>
        /// <param name="message">The message</param>
        /// <param name="type">The message type</param>
        public void Frenetic_SendMessage(string message, MessageType type)
        {
            string basecolor;
            switch (type)
            {
                case MessageType.BAD:
                    basecolor = TextStyle.Color_Outbad;
                    break;
                case MessageType.GOOD:
                    basecolor = TextStyle.Color_Outgood;
                    break;
                case MessageType.INFO:
                    basecolor = TextStyle.Color_Simple;
                    break;
                default:
                    basecolor = TextStyle.Color_Warning;
                    break;
            }
            SendMessage("[Remote] " + basecolor +
            ServerCommands.CommandSystem.TagSystem.ParseTags(message, basecolor, null, DebugMode.MINIMAL));
        }

        /// <summary>
        /// Ticks player movement.
        /// </summary>
        /// <param name="delta">The tick delta</param>
        /// <param name="custom">Whether tihis is a custom tick call (IE, anything outside the normal tick)</param>
        public void TickMovement(double delta, bool custom = false)
        {
            if (delta == 0)
            {
                return;
            }
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
            if (Downward)
            {
                Maxes.Z = 0.1;
            }
            else
            {
                Maxes.Z = 1.5;
            }
            bool on_ground = Collision.Box(InWorld, Position - (DefaultHalfSize + new Location(0, 0, 0.1)), Position + DefaultHalfSize) && Velocity.Z < 0.01;
            if (Upward)
            {
                if (on_ground && !Jumped)
                {
                    Velocity.Z += 10;
                    Jumped = true;
                }
            }
            else
            {
                Jumped = false;
            }
            if (movement.LengthSquared() > 0)
            {
                movement = Utilities.RotateVector(movement, Direction.X * Utilities.PI180);
            }
            float MoveSpeed = 15;
            Velocity.X += ((movement.X * MoveSpeed * (Slow || Downward ? 0.5 : 1)) - Velocity.X) * delta * 8;
            Velocity.Y += ((movement.Y * MoveSpeed * (Slow || Downward ? 0.5 : 1)) - Velocity.Y) * delta * 8;
            Velocity.Z += delta * -9.8 / 0.5666; // 1 unit = 0.5666 meters
            Location ppos = Position;
            Location target = Position + Velocity * delta;
            Location pos = Position;
            if (target != pos)
            {
                // TODO: Better handling (Based on impact normal)
                pos = Collision.BoxRayTrace(InWorld, -DefaultHalfSize, DefaultHalfSize, pos, new Location(target.X, target.Y, pos.Z), 1);
                pos = Collision.BoxRayTrace(InWorld, -DefaultHalfSize, DefaultHalfSize, pos, new Location(target.X, pos.Y, pos.Z), 1);
                pos = Collision.BoxRayTrace(InWorld, -DefaultHalfSize, DefaultHalfSize, pos, new Location(pos.X, target.Y, pos.Z), 1);
                pos = Collision.BoxRayTrace(InWorld, -DefaultHalfSize, DefaultHalfSize, pos, new Location(pos.X, pos.Y, target.Z), 1);
                Reposition(pos);
                Velocity = (pos - ppos) / delta;
            }
        }

        public List<MoveKeysPacketIn> PacketsToApply = new List<MoveKeysPacketIn>();

        public Chunk CurrentChunk = null;

        /// <summary>
        /// All chunk locations this player is aware of.
        /// </summary>
        public List<Location> ChunksAware = new List<Location>();

        /// <summary>
        /// What block the player has selected.
        /// </summary>
        public Location SelectedBlock;

        public override void Tick()
        {
            if (!IsValid)
            {
                return;
            }
            int pc = PacketsToApply.Count;
            for (int i = 0; i < pc; i++)
            {
                PacketsToApply[i].Apply();
            }
            PacketsToApply.RemoveRange(0, pc);
            TickMovement(ServerMain.Delta);
            // Manage Selection
            Location forward = Utilities.ForwardVector_Deg(Direction.X, Direction.Y);
            Location eye = Position + new Location(0, 0, Maxes.Z);
            Location seltarg = eye + forward * 10;
            SelectedBlock = Collision.BoxRayTrace(InWorld, new Location(-0.001), new Location(0.001), eye, seltarg, -1);
            if (SelectedBlock == seltarg)
            {
                SelectedBlock = Location.NaN;
            }
            if (Attack && !SelectedBlock.IsNaN())
            {
                Location sel_block = SelectedBlock.GetBlockLocation();
                Chunk ch = InWorld.LoadChunk(World.GetChunkLocation(SelectedBlock.GetBlockLocation()));
                if (ch.Blocks[(int)(sel_block.X - ch.X * 30), (int)(sel_block.Y - ch.Y * 30), (int)(sel_block.Z - ch.Z * 30)].Type != 0)
                {
                    ch.SetBlock((int)(sel_block.X - ch.X * 30), (int)(sel_block.Y - ch.Y * 30), (int)(sel_block.Z - ch.Z * 30), (ushort)Material.AIR);
                    InWorld.BroadcastBlock(sel_block);
                }
            }
            // TODO: Better tracking of what chunks to send
            List<Location> locs = GetChunksNear(World.GetChunkLocation(Position));
            foreach (Location loc in locs)
            {
                if (!ChunksAware.Contains(loc))
                {
                    ToSend.Add(loc);
                    ChunksAware.Add(loc);
                }
            }
            for (int i = 0; i < ChunksAware.Count; i++)
            {
                if (!locs.Contains(ChunksAware[i]))
                {
                    ToSend.Remove(ChunksAware[i]);
                    ChunksAware.RemoveAt(i--);
                }
            }
            sendtimer += ServerMain.Delta;
            if (sendtimer >= 0.05f)
            {
                sendtimer = 0.05f;
                if (ToSend.Count > 0)
                {
                    Chunk ch = InWorld.LoadChunk(ToSend[0]);
                    SendToSecondary(new ChunkPacketOut(ch));
                    for (int i = 0; i < ch.Entities.Count; i++)
                    {
                        if (ch.Entities[i] != this)
                        {
                            Send(new NewEntityPacketOut(ch.Entities[i]));
                        }
                    }
                    ToSend.RemoveAt(0);
                }
            }
            // Handle networking
            if (Network.received > 4)
            {
                while (true)
                {
                    int len = BitConverter.ToInt32(Network.recd, 0);
                    byte type = Network.recd[4];
                    if (Network.received - 5 >= len)
                    {
                        byte[] data = new byte[len];
                        if (len > 0)
                        {
                            Array.Copy(Network.recd, 5, data, 0, len);
                        }
                        Network.received -= 5 + len;
                        byte[] newdata = new byte[Network.Max];
                        if (Network.received > 0)
                        {
                            Array.Copy(Network.recd, 5 + len, newdata, 0, Network.received);
                        }
                        Network.recd = newdata;
                        AbstractPacketIn packet;
                        switch (type)
                        {
                            case 1:
                                packet = new PingPacketIn(this, false);
                                break;
                            case 2:
                                packet = new MoveKeysPacketIn(this, false);
                                break;
                            case 3:
                                packet = new CommandPacketIn(this, false);
                                break;
                            case 255:
                                packet = new DisconnectPacketIn(this, false);
                                return;
                            default:
                                Kick("Invalid packet " + (int)type);
                                return;
                        }
                        try
                        {
                            if (packet.ReadBytes(data))
                            {
                                packet.Apply();
                            }
                            else
                            {
                                Kick("Impure packet " + (int)type);
                            }
                        }
                        catch (Exception ex)
                        {
                            SysConsole.Output(OutputType.ERROR, "Networking / player / receive packet: " + ex.ToString());
                            Kick("Invalid packet " + (int)type);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
            if (ChunkNetwork.received > 4)
            {
                while (true)
                {
                    int len = BitConverter.ToInt32(ChunkNetwork.recd, 0);
                    byte type = ChunkNetwork.recd[4];
                    if (ChunkNetwork.received - 5 >= len)
                    {
                        byte[] data = new byte[len];
                        if (len > 0)
                        {
                            Array.Copy(ChunkNetwork.recd, 5, data, 0, len);
                        }
                        ChunkNetwork.received -= 5 + len;
                        byte[] newdata = new byte[ChunkNetwork.Max];
                        if (ChunkNetwork.received > 0)
                        {
                            Array.Copy(ChunkNetwork.recd, 5 + len, newdata, 0, ChunkNetwork.received);
                        }
                        ChunkNetwork.recd = newdata;
                        AbstractPacketIn packet;
                        switch (type)
                        {
                            case 1:
                                packet = new PingPacketIn(this, true);
                                break;
                            default:
                                Kick("Invalid packet " + (int)type);
                                return;
                        }
                        try
                        {
                            if (packet.ReadBytes(data))
                            {
                                packet.Apply();
                            }
                            else
                            {
                                Kick("Impure packet " + (int)type);
                            }
                        }
                        catch (Exception ex)
                        {
                            SysConsole.Output(OutputType.ERROR, "Networking / player / receive c-packet: " + ex.ToString());
                            Kick("Invalid packet " + (int)type);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        bool tdisco = false;

        /// <summary>
        /// Kicks the player with a given message.
        /// </summary>
        /// <param name="message">The kick message</param>
        public void Kick(string message)
        {
            tdisco = true;
            ServerMain.DespawnPlayer(this);
            Network.TickMe = false;
            ServerMain.Announce("Player " + Username + " disconnected: " + message);
            Send(new KickPacketOut(message));
            Network.InternalSocket.Close(2);
        }

        public void SendMessage(string message)
        {
            Send(new MessagePacketOut(message));
        }

        /// <summary>
        /// Send a packet along the primary socket.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void Send(AbstractPacketOut packet)
        {
            try
            {
                byte[] data = new byte[packet.Data.Length + 5];
                BitConverter.GetBytes(packet.Data.Length).CopyTo(data, 0);
                data[4] = packet.ID;
                packet.Data.CopyTo(data, 5);
                Network.InternalSocket.Send(data);
            }
            catch (Exception ex)
            {
                if (!tdisco)
                {
                    SysConsole.Output(OutputType.ERROR, "Error sending packet to player: " + ex.ToString());
                    Kick("Error sending packet");
                }
            }
        }

        /// <summary>
        /// Send a packet along the secondary socket.
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void SendToSecondary(AbstractPacketOut packet)
        {
            try
            {
                byte[] data = new byte[packet.Data.Length + 5];
                BitConverter.GetBytes(packet.Data.Length).CopyTo(data, 0);
                data[4] = packet.ID;
                packet.Data.CopyTo(data, 5);
                ChunkNetwork.InternalSocket.Send(data);
            }
            catch (Exception ex)
            {
                if (!tdisco)
                {
                    SysConsole.Output(OutputType.ERROR, "Error sending packet to player (secondary): " + ex.ToString());
                    Kick("Error sending secondary packet");
                }
            }
        }

        static List<Location> GetChunksNear(Location pos)
        {
            List<Location> chunks = new List<Location>();
            // TODO: Spiral algorithm? Perhaps trace the player's forward look direction and spawn chunks in a cone that direction!
            chunks.Add(pos + new Location(0, 0, 0));
            chunks.Add(pos + new Location(0, 0, -1));
            chunks.Add(pos + new Location(0, 0, -2));
            chunks.Add(pos + new Location(0, 0, 1));
            chunks.Add(pos + new Location(0, 0, 2));
            chunks.Add(pos + new Location(0, 1, 0));
            chunks.Add(pos + new Location(1, 0, 0));
            chunks.Add(pos + new Location(1, 1, 0));
            chunks.Add(pos + new Location(0, 1, 1));
            chunks.Add(pos + new Location(1, 0, 1));
            chunks.Add(pos + new Location(1, 1, 1));
            chunks.Add(pos + new Location(0, -1, 0));
            chunks.Add(pos + new Location(-1, 0, 0));
            chunks.Add(pos + new Location(1, 1, 0));
            chunks.Add(pos + new Location(0, 1, -1));
            chunks.Add(pos + new Location(-1, 0, -1));
            chunks.Add(pos + new Location(-1, -1, -1));
            chunks.Add(pos + new Location(0, 2, 0));
            chunks.Add(pos + new Location(0, -2, 0));
            chunks.Add(pos + new Location(-2, 0, 0));
            chunks.Add(pos + new Location(2, 0, 0));
            for (int x = -3; x < 4; x++)
            {
                for (int y = -3; y < 4; y++)
                {
                    for (int z = -3; z < 4; z++)
                    {
                        Location ch = pos + new Location(x, y, z);
                        if (!chunks.Contains(ch))
                        {
                            chunks.Add(ch);
                        }
                    }
                }
            }
            return chunks;
        }

        List<Location> ToSend = new List<Location>();

        double sendtimer = 0;
    }
}
