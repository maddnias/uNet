using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using uNet.Structures;
using uNet.Structures.Packets;

namespace uNet.Server
{
    public class uNetServer
    {
        #region Events
        /// <summary>
        /// Invoked when a new peer connects
        /// </summary>
        public event PeerEventHandler OnPeerConnected;
        /// <summary>
        /// Invoked when a peer loses connection
        /// </summary>
        public event PeerEventHandler OnPeerDisconnected;
        /// <summary>
        /// Invoked when a packet is received from a remote peer
        /// </summary>
        public event PacketEventHandler OnPacketReceived;
        /// <summary>
        /// Invoked when a packet is sent from local server
        /// </summary>
        public event PacketEventHandler OnPacketSent;
        #endregion

        #region Public fields
        public List<Peer> ConnectedPeers { get; set; }
        public static ServerSettings Settings { get; private set; }
        #endregion

        #region Private fields
        private readonly IPEndPoint _endPoint;
        private readonly TcpListener _uNetSock;
        private readonly bool _debug;
        private readonly object _sendLock = new object();
        #endregion

        public uNetServer(uint port, string address = "0.0.0.0", bool debug = false)
        {
            _uNetSock = new TcpListener(IPAddress.Parse(address), (int)port);
            _endPoint = new IPEndPoint(IPAddress.Parse(address), (int)port);
            _debug = debug;
            ConnectedPeers = new List<Peer>();

            Settings = new ServerSettings(new List<IPacket>(), false);
        }

        public uNetServer(uint port, ServerSettings settings, string address = "0.0.0.0", bool debug = false)
        {
            _uNetSock = new TcpListener(IPAddress.Parse(address), (int)port);
            _endPoint = new IPEndPoint(IPAddress.Parse(address), (int)port);
            _debug = debug;
            ConnectedPeers = new List<Peer>();

            Settings = settings;
        }

        /// <summary>
        /// Initialize the server and start listening
        /// </summary>
        public void Initialize()
        {
            _uNetSock.Start(100);
            AcceptAsync();
        }

        /// <summary>
        /// Broadcast a packet to all currently connected peers
        /// </summary>
        /// <param name="packet">The packet to be broadcasted</param>
        public void Broadcast(IPacket packet)
        {
            ConnectedPeers.ForEach(x => 
                {
                    lock (_sendLock)
                    {
                        x.Processor.SendPacket(packet, x.NetStream).Wait();
                    }
                });
        }

        /// <summary>
        /// Sends a packet containing data to a specific peer
        /// </summary>
        /// <param name="packet">The packet to be sent</param>
        /// <param name="targetPeer">A peer will be filtered from connected peers based on this predicate</param>
        public void SendPacket(IPacket packet, Predicate<Peer> targetPeer)
        {
            var target = ConnectedPeers.FirstOrDefault(x => targetPeer(x));

            if (target == null)
                throw new Exception("No peer match for current predicate");

            lock (_sendLock) target.Processor.SendPacket(packet, target.NetStream).Wait();
        }

        #region Internal socket operations
        private void PeerDisconnect(object sender, PeerEventArgs e)
        {
            if (OnPeerDisconnected != null)
                OnPeerDisconnected(null, e);

            ConnectedPeers.Remove(e.Peer);

            if (_debug)
                Debug.Print("Peer disconnected from: {0} with reason: {1}", e.Peer.RemoteEndPoint,
                            ((PeerDisconnectedEventArgs) e).DisconnectReason);
        }
        private async void AcceptAsync()
        {
            while (true)
            {
                var client = await _uNetSock.AcceptTcpClientAsync();
                var peer = new Peer(client, this, Settings);

                //Subscribe to peer events
                peer.OnPeerDisconnected += PeerDisconnect;
                peer.OnPacketReceived += (o, e) => { if(OnPacketReceived != null) { OnPacketReceived(o, e); } };
                peer.InternalOnPacketReceived += InternalOnPacketReceived;
                peer.Processor.OnPacketSent += (o, e) => { if (OnPacketSent != null) { OnPacketSent(null, new PacketEventArgs(null, e.Packet, e.RawPacketSize)); } }; 

                ConnectedPeers.Add(peer);

                if (_debug)
                    Debug.Print("Peer connected from: " + peer.RemoteEndPoint);

                if (OnPeerConnected != null)
                    OnPeerConnected(null, new PeerConnectedEventArgs(peer));
            }
        }
        /// <summary>
        /// Used for uProtocol specific packets which should not be handled by user
        /// </summary>
        internal void InternalOnPacketReceived(object sender, PacketEventArgs e)
        {
            Console.WriteLine("Received {0} bytes", e.RawPacketSize);

            switch (e.Packet.ID)
            {
                case 9998:
                    if (((HandshakePacket) e.Packet).Version != Globals.Version)
                    {
                        SendPacket(new ErrorPacket("Protocol version mismatch"), x => x == e.SourcePeer);
                        e.SourcePeer.Disconnect(string.Format("Protocol version mismatch. (Local:{0}/Remote:{1})", Globals.Version, (e.Packet as HandshakePacket).Version));
                    }
                
                    if (
                        !((HandshakePacket) e.Packet).CustomPackets.TrueForAll(
                            x =>
                            Globals.ReservedPacketIDs.Contains(x) ||
                            Settings.PacketTable.FirstOrDefault(p => p.ID == x) != null))
                    {
                        SendPacket(new ErrorPacket("Packet table mismatch"), x => x == e.SourcePeer);
                        e.SourcePeer.Disconnect("Packet table mismatch");
                    }

                    break;
            }
        }
        #endregion
    }

}
