using uNet.Server;

namespace uNet.Structures.Events
{
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
}