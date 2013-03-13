using System;
using System.Collections.Generic;
using System.IO;
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
    public class Peer : IDisposable
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

        public Peer(TcpClient client, uNetServer server, ServerSettings settings, PacketProcessor processor)
        {
            Client = client;
            RemoteEndPoint = Client.Client.RemoteEndPoint;
            BufferSize = settings.ReceiveBufferSize;
            Processor = processor;

            if (settings.UseSsl)
            {
                NetStream = new SslStream(Client.GetStream(), true);
                (NetStream as SslStream).AuthenticateAsServer(
                    new X509Certificate(File.ReadAllBytes(settings.SslCertLocation)));
            }
            else
                NetStream = Client.GetStream();

            Server = server;

            ReadAsync();
        }

        public void Disconnect(string reason = "")
        {
            Dispose();

            if (OnPeerDisconnected != null)
                OnPeerDisconnected(null, new PeerDisconnectedEventArgs(this, reason));
        }

        internal async void ReadAsync()
        {
            var fBuff = new List<byte>();
            var tmpBuff = new byte[BufferSize];

            while (true)
            {
                try
                {
                    var read = await NetStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);

                    if (read == 0)
                    {
                        Disconnect();
                        break;
                    }

                    fBuff.AddRange(tmpBuff.Slice(0, read));

                    Processor.ProcessData(fBuff).ForEach(packet =>
                                                             {
                                                                 if (
                                                                     Globals.ReservedPacketIDs.Contains(
                                                                         packet.ID))
                                                                     InternalOnPacketReceived(null,
                                                                                              new PacketEventArgs(null,
                                                                                                                  packet));
                                                                 else
                                                                     OnPacketReceived(null,
                                                                                      new PacketEventArgs(null,
                                                                                                          packet));
                                                             });
                }
                catch
                {
                    Disconnect();
                    break;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
                if (Client != null)
                {
                    NetStream.Dispose();
                    Client.Close();
                }
        }
    }
}