using System;
using uNet.Server;

namespace uNet.Structures.Events
{
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
}