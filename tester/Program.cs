using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uNet;
using uNet.Client;
using uNet.Structures;
using uNet.Structures.Compression;
using uNet.Structures.Packets;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings;

namespace tester
{
    class Program
    {
        static void Main(string[] args)
        {
            var settings = new ClientSettings()
            {
                PacketTable = null,
                PacketCompressor = new LZ4Compressor(),
                UseSsl = false
            };

            Thread.Sleep(2000);
            var cl = new uNetClient("127.0.0.1", 1337, settings);

            cl.OnPacketSent += (o, e) => Console.WriteLine("Sent {0} bytes...", e.RawPacketSize);
            cl.OnPacketReceived += (o, e) => Console.WriteLine("Received {0} bytes...", e.RawPacketSize);
            cl.OnConnected += (o, e) => Console.WriteLine("Successfully connected to host");
            cl.OnDisconnected += (o, e) => Console.WriteLine("Lost connection");

            cl.Connect();

            Console.ReadLine();
        }
    }
}
