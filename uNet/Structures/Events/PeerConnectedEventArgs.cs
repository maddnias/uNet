using uNet.Server;

namespace uNet.Structures.Events
{
    public class PeerConnectedEventArgs : PeerEventArgs
    {
        public PeerConnectedEventArgs(Peer peer)
            : base(peer)
        {
        }
    }
}