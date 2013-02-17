using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using uNet.Structures.Events;
using uNet.Structures.Settings;
using uNet.Utilities;
using uNet.Utilities.Extensions;

namespace uNet.Server
{
    public class Peer
    {
        internal event PeerEventHandler OnPeerDisconnected;
        internal event PacketEventHandler OnPacketReceived;
        internal event PacketEventHandler InternalOnPacketReceived;
        internal TcpClient Client { get; set; }
        internal PacketProcessor Processor { get; set; }
        internal Stream NetStream;
        internal uNetServer Server { get; private set; }

        public EndPoint RemoteEndPoint { get; set; }
        public int BufferSize { get; set; }


        public Peer(TcpClient client, uNetServer server, ServerSettings settings)
        {
            Client = client;
            RemoteEndPoint = Client.Client.RemoteEndPoint;
            BufferSize = settings.ReceiveBufferSize;

            if (settings.UseSsl)
            {
                NetStream = new SslStream(Client.GetStream(), true);
                (NetStream as SslStream).AuthenticateAsServer(new X509Certificate(File.ReadAllBytes(settings.SslCertLocation)));
            }
            else
                NetStream = Client.GetStream();

            Processor = new PacketProcessor(uNetServer.Settings);
            Server = server;

            ReadAsync();
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

                            var parsedPacket = Processor.ParsePacket(fBuff.Take(packetSize).ToArray());
                            fBuff.RemoveRange(0, packetSize);

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
