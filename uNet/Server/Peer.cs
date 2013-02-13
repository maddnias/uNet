using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using uNet.Tools;
using uNet.Tools.Extensions;

namespace uNet.Server
{
    public class Peer
    {
        internal event PeerEventHandler OnPeerDisconnected;
        internal event PacketEventHandler OnPacketReceived;
        internal event PacketEventHandler InternalOnPacketReceived;
        internal TcpClient Client { get; set; }
        internal PacketProcessor Processor { get; set; }
        internal NetworkStream NetStream;
        internal uNetServer Server { get; private set; }

        public EndPoint RemoteEndPoint { get; set; }
        public int BufferSize { get; set; }


        public Peer(TcpClient client, uNetServer server)
        {
            Client = client;
            RemoteEndPoint = Client.Client.RemoteEndPoint;
            BufferSize = 1024;
            NetStream = Client.GetStream();
            Processor = new PacketProcessor(uNetServer.Settings);
            Server = server;

            //ReadAsync();
        }

        public void Disconnect(string reason = "")
        {
            NetStream.Dispose();
            Client.Close();

            if(OnPeerDisconnected != null)
                OnPeerDisconnected(null, new PeerDisconnectedEventArgs(this, reason));
        }

        internal async void ReadAsync()
        {
            var fBuff = new List<byte>();
            var tmpBuff = new byte[BufferSize];

            while (true)
            {
                if (!Client.Connected)
                    return;

                if (NetStream.CanRead)
                {
                    try
                    {
                        var read = await NetStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);
                        fBuff.AddRange(tmpBuff.Slice(0, read));

                        while (true)
                        {
                            if (fBuff.Count < sizeof(int)) break;

                            var packetSize = BitConverter.ToInt32(fBuff.ToArray(), 0);

                            if (fBuff.Count < packetSize + sizeof (int))
                                break;

                            // Remove length prefix
                            fBuff.RemoveRange(0, sizeof (int));

                            bool verified;
                            var parsedPacket = Processor.ParsePacket(fBuff.Take(packetSize).ToArray(), out verified);
                            fBuff.RemoveRange(0, packetSize);

                            if (!verified)
                            {
                                Disconnect("Packet hash verification failed.");
                                break;
                            }

                            if (OnPacketReceived != null)
                            {
                                var source =
                                    Server.ConnectedPeers.FirstOrDefault(
                                        x => x.RemoteEndPoint == Client.Client.RemoteEndPoint);

                                // don't invoke onpacketreceived on packets used for internal protocol
                                if (Globals.ReservedPacketIDs.Contains(parsedPacket.ID))
                                    InternalOnPacketReceived(null,
                                                             new PacketEventArgs(source, parsedPacket,
                                                                                 packetSize + sizeof (int)));
                                else
                                    OnPacketReceived(null,
                                                     new PacketEventArgs(source, parsedPacket,
                                                                         packetSize + sizeof (int)));
                            }

                            if (fBuff.Count < sizeof (int))
                                break;
                        }
                    }
                    catch
                    {
                        Disconnect();
                    }

                }
                else
                    Disconnect();
            }
        }
    }
}
