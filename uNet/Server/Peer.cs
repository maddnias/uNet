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

        public EndPoint RemoteEndPoint { get; set; }
        public int BufferSize { get; set; }


        public Peer(TcpClient client)
        {
            Client = client;
            RemoteEndPoint = Client.Client.RemoteEndPoint;
            BufferSize = 1024;
            NetStream = Client.GetStream();
            Processor = new PacketProcessor();

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
            while (true)
            {
                if (!Client.Connected)
                    return;

                if (NetStream.CanRead)
                {
                    var fBuff = new List<byte>();
                    var tmpBuff = new byte[BufferSize];
                    int packetSize;

                    // calculate packet size before attempting to receive
                    try{
                        packetSize = await Processor.CalculatePrefix(NetStream);

                        if (packetSize == -1){
                            Disconnect();
                            return;
                        }
                    } catch {
                        Disconnect();
                        return;
                    }

                    while (true)
                    {
                        var read = await NetStream.ReadAsync(tmpBuff, 0, tmpBuff.Length);

                        if (read == 0)
                            Disconnect();

                        // slice redundant null bytes off of the buffer
                        fBuff.AddRange((read >= BufferSize ? tmpBuff : tmpBuff.Slice(0, read)));

                        // continue reading until buffer reaches complete packet size
                        if (fBuff.Count >= packetSize)
                            break;
                    }

                    if (OnPacketReceived != null)
                    {
                        var source = Globals.Server.ConnectedPeers.FirstOrDefault(x => x.RemoteEndPoint == Client.Client.RemoteEndPoint);
                        var parsedPacket = Processor.ParsePacket(fBuff.ToArray());

                        if (source == null)
                            throw new uNetPeerException("Could not retrieve source peer");

                        // don't invoke onpacketreceived on packets used for internal protocol
                        if (Globals.ReservedPacketIDs.Contains(parsedPacket.ID))
                            InternalOnPacketReceived(null, new PacketEventArgs(source, parsedPacket, fBuff.Count + sizeof(int)));
                        else
                            OnPacketReceived(null, new PacketEventArgs(source, parsedPacket, fBuff.Count + sizeof(int)));
                    }
                }
                else
                    Disconnect();
            }
        }
    }
}
