using System;
using uNet.Server;
using uNet.Structures.Compression;
using uNet.Structures.Settings;

namespace TestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new ServerSettings
                              {
                                  PacketTable = null,
                                  PacketCompressor = new LZ4Compressor(),
                                  UseSsl = false
                              };
 
            var srv = new uNetServer(1337, settings);

            srv.Initialize();

            srv.OnPeerConnected += (o, e) => Console.WriteLine("Peer connected from: " + e.Peer.RemoteEndPoint);
            srv.OnPeerDisconnected += (o, e) => Console.WriteLine("Peer disconnected from: " + e.Peer.RemoteEndPoint);
            srv.OnPacketReceived += (o, e) => Console.WriteLine("Received {0} bytes", e.Packet.PacketSize);
            srv.OnPacketSent += (o, e) => Console.WriteLine("Sent packet with ID: " + e.Packet.ID + " and size: " + e.Packet.PacketSize);

            Console.ReadLine();
        }
    }
}
