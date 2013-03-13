using System.Collections.Generic;
using System.IO;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings.Base;

namespace uNet.Structures.Packets
{
    /// <summary>
    /// First packet sent on connection to verify that both peer and server uses same version of uProtocol
    /// </summary>
    public class HandshakePacket : INonCompressedPacket
    {
        public short ID { get { return 9998; } }

        public string Version { get; set; }
        public List<short> CustomPackets { get; set; }
        public short CompressorID { get; set; }

        public int PacketSize
        {
            get { return sizeof (short) + Version.Length + (CustomPackets.Count*sizeof (short)) + sizeof (short); }
        }

        public HandshakePacket()
        {
            CustomPackets = new List<short>();
        }

        public HandshakePacket(OptionSet settings)
        {
            Version = Globals.Version;
            CustomPackets = new List<short>();

            (settings.PacketTable ?? new List<IPacket>()).ForEach(x => CustomPackets.Add(x.ID));

            if (settings.PacketCompressor == null)
                CompressorID = -1;
            else
                CompressorID = settings.PacketCompressor.CompressionID;
        }

        public void SerializePacket(Stream outputBuffer)
        {
            using (var writer = new BinaryWriter(outputBuffer))
            {
                writer.Write(Version);
                writer.Write(CompressorID);
                writer.Write(CustomPackets.Count);

                CustomPackets.ForEach(writer.Write);
            }
        }

        public void DeserializePacket(Stream inputBuffer)
        {
            using (var reader = new BinaryReader(inputBuffer))
            {
                Version = reader.ReadString();
                CompressorID = reader.ReadInt16();

                var packetCount = reader.ReadInt32();

                for (var i = 0; i < packetCount; i++)
                    CustomPackets.Add(reader.ReadInt16());
            }
        }
    }
}
