using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using uNet.Structures.Events;
using uNet.Structures.Packets.Base;
using uNet.Utilities.Extensions;

namespace uNet.Utilities
{
    public class uProtocolProcessor : PacketProcessor
    {
        public override async Task SendPacket(IPacket packet, Stream netStream)
        {
            var ms = new MemoryStream();
            var bw = new BinaryWriter(ms);
            var compressFlag = false;

            if (packet is IAutoSerializePacket)
                (packet as IAutoSerializePacket).AutoSerialize(bw);
            else
            {
                bw.Write(packet.ID);
                packet.SerializePacket(ms);
            }

            bw.Flush();

            if (Settings.PacketCompressor != null ||
                (Settings.PacketCompressor != null && packet is IForceCompressedPacket))
                compressFlag = true;

            if (packet is INonCompressedPacket)
                compressFlag = false;

            // Copy ms -> redirect writer to new ms -> prepend packet size prefix -> append packet paylod
            FinalizePacket(ref bw, compressFlag, packet);
            ms.Dispose(); // Dispose of expired ms, writer's basestream is created in FinalizePacket
            ms = bw.BaseStream as MemoryStream;

            await netStream.WriteAsync(ms.ToArray(), 0, (int) ms.Length);

            OnPacketSent(new PacketEventArgs(null, packet));

            ms.Dispose();
            bw.Dispose();

        }

        private void FinalizePacket(ref BinaryWriter writer, bool compress, IPacket packet)
        {
            var data = ((MemoryStream) writer.BaseStream).ToArray();
            var ms = new MemoryStream();
            writer = new BinaryWriter(ms);
            byte[] compressedData = null;

            if (compress)
                compressedData = Settings.PacketCompressor.CompressData(data);

            if (compressedData != null)
            {
                if (compressedData.Length > data.Length && !(packet is IForceCompressedPacket))
                    compressedData = null;
                else
                    data = compressedData;
            }

            // Prepend packet size prefix
            writer.Write(data.Length + 1);
            writer.Write((byte) (compress && compressedData != null ? 0x1 : 0x0));
            writer.Write(data);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        public override IPacket ParsePacket(byte[] rawData)
        {
            IPacket packet;

            // Compression flag
            if (rawData[0] == 0x1)
                rawData = Settings.PacketCompressor.DecompressData(rawData.Slice(1, rawData.Length - 1));
            else
                rawData = rawData.Slice(1, rawData.Length - 1);

            using (var ms = new MemoryStream(rawData))
            using (var br = new BinaryReader(ms))
            {
                var id = br.ReadInt16();
                // Find and create instance of packet from table depending on ID
                var packetType = Settings.PacketTable.FirstOrDefault(x => x.ID == id).GetType();
                packet = Activator.CreateInstance(packetType) as IPacket;

                // Populate packet fields
                packet.DeserializePacket(ms);
            }

            return packet;
        }

        public override List<IPacket> ProcessData(List<byte> buffer)
        {
            var parsedPackets = new List<IPacket>();

            while (true)
            {
                if (buffer.Count < sizeof(int)) break;

                var packetSize = BitConverter.ToInt32(buffer.ToArray(), 0);

                if (buffer.Count < packetSize + sizeof(int))
                    break;

                // Remove length prefix
                buffer.RemoveRange(0, sizeof(int));

                parsedPackets.Add(ParsePacket(buffer.Take(packetSize).ToArray()));
                buffer.RemoveRange(0, packetSize);

                if (buffer.Count < sizeof(int))
                    break;
            }

            return parsedPackets;
        }
    }
}