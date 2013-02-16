using uNet.Structures.Packets.Base;

namespace uNet.Structures.Packets
{
    public class ExamplePacket : IAutoSerializePacket, IEncryptedPacket
    {
        public short ID
        {
            get { return 0; }
        }

        public void SerializePacket(System.IO.BinaryWriter writer) { }

        public void DeserializePacket(System.IO.BinaryReader reader)
        {
            
        }
    }
}
