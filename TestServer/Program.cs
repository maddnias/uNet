using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uNet;
using uNet.Server;
using uNet.Structures;
using uNet.Structures.Packets;
using uNet.Structures.Schemes;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var customPackets = new List<IPacket> 
            {
                new ExamplePacket()
            };

            var srv = new uNetServer(1337, new OptionSet(true, new TestScheme(), customPackets));

            srv.Initialize();

            srv.OnPeerConnected += (o, e) => Console.WriteLine("Peer connected from: " + e.Peer.RemoteEndPoint);
            srv.OnPeerDisconnected += (o, e) => Console.WriteLine("Peer disconnected from: " + e.Peer.RemoteEndPoint);
            srv.OnPacketReceived += (o, e) => Console.WriteLine("Received {0} bytes", e.RawPacketSize);
            srv.OnPacketSent += (o, e) => Console.WriteLine("Sent packet with ID: " + e.Packet.ID + " and size: " + e.RawPacketSize);

            Console.ReadLine();
        }
    }
}
