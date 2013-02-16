using System.Collections.Generic;
using uNet.Structures.Packets.Base;

namespace uNet.Structures.Packets
{
    /// <summary>
    /// First packet sent on connection to verify that both peer and server uses same version of uProtocol
    /// </summary>
    public class HandshakePacket : IEncryptedPacket
    {
        public short ID { get { return 9998; } }
        public string Version { get; set; }
        public List<short> CustomPackets { get; set; }

        public HandshakePacket()
        {
            CustomPackets = new List<short>();
        }

        public HandshakePacket(OptionSet settings)
        {
            Version = Globals.Version;
            CustomPackets = new List<short>();

            (settings.PacketTable ?? new List<IPacket>()).ForEach(x => CustomPackets.Add(x.ID));
        }

        public void SerializePacket(System.IO.BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(CustomPackets.Count);

            CustomPackets.ForEach(writer.Write);
        }

        public void DeserializePacket(System.IO.BinaryReader reader)
        {
            Version = reader.ReadString();

            var packetCount = reader.ReadInt32();

            for (var i = 0; i < packetCount; i++)
                CustomPackets.Add(reader.ReadInt16());
        }
    }
}
