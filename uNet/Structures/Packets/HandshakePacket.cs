namespace uNet.Structures.Packets
{
    /// <summary>
    /// First packet sent on connection to verify that both peer and server uses same version of uProtocol
    /// </summary>
    public class HandshakePacket : IAutoSerializePacket
    {
        public short ID { get { return 0; } }
        public string Version { get; set; }
        public int CryptoSchemeID { get; set; }
        public bool VerifyPackets { get; set; }

        public HandshakePacket()
        {
            Version = Globals.Version;
            CryptoSchemeID = Globals.Settings.CryptoScheme == null ? 0 : Globals.Settings.CryptoScheme.SchemeID;
            VerifyPackets = Globals.Settings.VerifyPackets;
        }

        public void SerializePacket(System.IO.BinaryWriter writer)
        {
        }

        public void DeserializePacket(System.IO.BinaryReader reader)
        {
            Version = reader.ReadString();
            CryptoSchemeID = reader.ReadInt32();
            VerifyPackets = reader.ReadBoolean();
        }
    }
}
