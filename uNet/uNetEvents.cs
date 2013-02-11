using System;
using uNet.Server;
using uNet.Structures;

namespace uNet
{
    #region Client
    public delegate void ClientEventHandler(object sender, ClientEventArgs e);

    public class ClientEventArgs : EventArgs
    {
    }
    #endregion

    #region Server
    public delegate void PeerEventHandler(object sender, PeerEventArgs e);
    public delegate void PacketEventHandler(object sender, PacketEventArgs e);

    /// <summary>
    /// Arguments associated with peer events
    /// </summary>
    public class PeerEventArgs : EventArgs 
    { 
        /// <summary>
        /// Associated peer
        /// </summary>
        public Peer Peer { get; set; }

        public PeerEventArgs(Peer peer)
        {
            Peer = peer;
        }
    }

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
        /// <summary>
        /// Raw size of data received/sent, including packet prefix and additional formatting
        /// </summary>
        public int RawPacketSize { get; set; }

        public PacketEventArgs(Peer peer, IPacket packet, int rawPacketSize)
        {
            SourcePeer = peer;
            Packet = packet;
            RawPacketSize = rawPacketSize;
        }
    }

    public class PeerConnectedEventArgs : PeerEventArgs
    {
        public PeerConnectedEventArgs(Peer peer)
            : base(peer)
        {
        }
    }
    public class PeerDisconnectedEventArgs : PeerEventArgs
    {
        /// <summary>
        /// Contains reason for disconnect when possible
        /// </summary>
        public string DisconnectReason { get; set; }

        public PeerDisconnectedEventArgs(Peer peer, string reason)
            : base(peer)
        {
            DisconnectReason = reason;
        }
    }
    #endregion
}
