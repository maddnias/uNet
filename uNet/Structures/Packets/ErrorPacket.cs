namespace uNet.Structures.Packets
{
    /// <summary>
    /// Sent whenever there's a error regarding a peer
    /// </summary>
    public class ErrorPacket : IAutoSerializePacket, IEncryptedPacket
    {
        public short ID { get { return 9999; } }
        public string ErrorMessage { get; set; }

        public ErrorPacket()
        {

        }

        public ErrorPacket(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public void SerializePacket(System.IO.BinaryWriter writer) { }

        public void DeserializePacket(System.IO.BinaryReader reader)
        {
            ErrorMessage = reader.ReadString();
        }
    }
}
