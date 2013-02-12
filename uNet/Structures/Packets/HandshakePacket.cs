namespace uNet.Structures.Packets
{
    /// <summary>
    /// First packet sent on connection to verify that both peer and server uses same version of uProtocol
    /// </summary>
    public class HandshakePacket : IAutoSerializePacket, IEncryptedPacket
    {
        public short ID { get { return 0; } }
        public string Version { get; set; }
        public int CryptoSchemeID { get; set; }

        public HandshakePacket()
        {
            Version = Globals.Version;
            CryptoSchemeID = Globals.CryptoScheme == null ? 0 : Globals.CryptoScheme.SchemeID;
        }

        public void SerializePacket(System.IO.BinaryWriter writer)
        {
        }

        public void DeserializePacket(System.IO.BinaryReader reader)
        {
            Version = reader.ReadString();
            CryptoSchemeID = reader.ReadInt32() + 55;
        }
    }
}
