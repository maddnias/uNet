using System.IO;
using uNet.Structures.Packets.Base;

namespace uNet.Structures.Packets
{
    /// <summary>
    /// Sent whenever there's a error regarding a peer
    /// </summary>
    public class ErrorPacket : IAutoSerializePacket, INonCompressedPacket
    {
        public short ID { get { return 9999; } }
        public string ErrorMessage { get; set; }

        public int PacketSize
        {
            get { return sizeof (short) + ErrorMessage.Length; }

        }

        public ErrorPacket()
        {

        }

        public ErrorPacket(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        public void SerializePacket(Stream outputBuffer) { }

        public void DeserializePacket(Stream inputBuffer)
        {
            using(var reader = new BinaryReader(inputBuffer))
                ErrorMessage = reader.ReadString();
        }
    }
}
