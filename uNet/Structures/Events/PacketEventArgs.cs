using System;
using uNet.Server;
using uNet.Structures.Packets.Base;

namespace uNet.Structures.Events
{
    /// <summary>
    /// Arguments assoicated with packet events
    /// </summary>
    public class PacketEventArgs : EventArgs
    {
        /// <summary>
        /// Peer from which the packet came from, null if packet was sent from local
        /// </summary>
        public Peer SourcePeer { get; set; }
        /// <summary>
        /// Associated packet
        /// </summary>
        public IPacket Packet { get; set; }

        public PacketEventArgs(Peer peer, IPacket packet)
        {
            SourcePeer = peer;
            Packet = packet;
        }
    }
}