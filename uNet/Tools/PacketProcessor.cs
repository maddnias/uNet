using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using uNet.Structures;
using uNet.Structures.Packets;
using uNet.Tools;
using uNet.Tools.Extensions;

namespace uNet.Tools
{
    internal class PacketProcessor
    {
        internal event PacketEventHandler OnPacketSent;

        private List<IPacket> _packetTable = new List<IPacket> 
        {
            new HandshakePacket(),
            new ErrorPacket(),
        };

        public async void SendPacket(IPacket packet, NetworkStream netStream)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);

            if (packet is IAutoSerializePacket)
                (packet as IAutoSerializePacket).AutoSerialize(bw);
            else
            {
                packet.SerializePacket(bw);
                bw.Write(packet.ID);
            }

            bw.Flush();

            // Copy ms -> redirect writer to new ms -> prepend packet size prefix -> append packet paylod
            FinalizePacket(ref bw);
            ms.Dispose(); // Dispose of expired ms, writer's basestream is created in FinalizePacket
            ms = bw.BaseStream as MemoryStream;

            await netStream.WriteAsync(ms.ToArray(), 0, (int)ms.Length);

            if (OnPacketSent != null)
                OnPacketSent(null, new PacketEventArgs(null, packet, (int)ms.Length));

            ms.Dispose();
            bw.Dispose();

        }

        private void FinalizePacket(ref BinaryWriter writer)
        {
            var data = (writer.BaseStream as MemoryStream).ToArray();
            var ms = new MemoryStream();
            writer = new BinaryWriter(ms);

            // Prepend packet size prefix
            writer.Write(data.Length);
            writer.Write(data);
        }

        public IPacket ParsePacket(byte[] rawData)
        {
            IPacket packet;

            using(var ms = new MemoryStream(rawData))
            using (var br = new BinaryReader(ms))
            {
                var id = br.ReadInt16();
                // Find and create instance of packet from table depending on ID
                var packetType = _packetTable.FirstOrDefault(x => x.ID == id).GetType();
                packet = Activator.CreateInstance(packetType) as IPacket;

                if (packet == null) // ID not present in table
                    throw new Exception("Unknown packet received!");

                // Populate packet fields
                packet.DeserializePacket(br);
            }

            return packet;
        }

        public async Task<int> CalculatePrefix(NetworkStream netStream)
        {
            var packetSize = new List<byte>(sizeof(int));
            var tmpBuff = new byte[1];

            while (true)
            {
                // receive 1 byte at a time, ensuring complete packet
                var read = await netStream.ReadAsync(tmpBuff, 0, 1);

                if (read == 0)
                    return -1;

                packetSize.AddRange(tmpBuff);

                // if entire integer has been read then return calculated packet size
                if (packetSize.Count >= sizeof(int))
                    return BitConverter.ToInt32(packetSize.ToArray(), 0);
            }
        }
    }
}
