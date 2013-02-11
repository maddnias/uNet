using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uNet;
using uNet.Server;
using uNet.Structures.Packets;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {

            var srv = new uNetServer(1337, "0.0.0.0", true);
            srv.Initialize();

            srv.OnPeerConnected += (o, e) => Console.WriteLine("Peer connected from: " + e.Peer.RemoteEndPoint);
            srv.OnPeerDisconnected += (o, e) => Console.WriteLine("Peer disconnected from: " + e.Peer.RemoteEndPoint);
            srv.OnPacketReceived += (o, e) => Console.WriteLine("Message: " + ((HandshakePacket) e.Packet).Version);
            srv.OnPacketSent += (o, e) => Console.WriteLine("Sent packet with ID: " + e.Packet.ID + " and size: " + e.RawPacketSize);

            Console.ReadLine();
        }
    }
}
