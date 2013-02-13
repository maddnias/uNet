using System.Collections.Generic;
using uNet.Server;
using uNet.Tools;

namespace uNet.Structures.Packets
{
    /// <summary>
    /// First packet sent on connection to verify that both peer and server uses same version of uProtocol
    /// </summary>
    public class HandshakePacket : IEncryptedPacket
    {
        public short ID { get { return 9998; } }
        public string Version { get; set; }
        public int CryptoSchemeID { get; set; }
        public bool VerifyPackets { get; set; }
        public List<short> CustomPackets { get; set; }

        public HandshakePacket()
        {
            CustomPackets = new List<short>();
        }

        public HandshakePacket(OptionSet settings)
        {
            Version = Globals.Version;
            CryptoSchemeID = settings.CryptoScheme == null ? 0 : settings.CryptoScheme.SchemeID;
            VerifyPackets = settings.VerifyPackets;
            CustomPackets = new List<short>();

            settings.PacketTable.ForEach(x => CustomPackets.Add(x.ID));
        }

        public void SerializePacket(System.IO.BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(CryptoSchemeID);
            writer.Write(VerifyPackets);
            writer.Write(CustomPackets.Count);

            CustomPackets.ForEach(writer.Write);
        }

        public void DeserializePacket(System.IO.BinaryReader reader)
        {
            Version = reader.ReadString();
            CryptoSchemeID = reader.ReadInt32();
            VerifyPackets = reader.ReadBoolean();

            var packetCount = reader.ReadInt32();

            for (var i = 0; i < packetCount; i++)
                CustomPackets.Add(reader.ReadInt16());
        }
    }
}
