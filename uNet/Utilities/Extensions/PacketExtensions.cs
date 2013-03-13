using System.IO;
using uNet.Structures.Packets.Base;

namespace uNet.Utilities.Extensions
{
    public static class PacketExtensions
    {
        /// <summary>
        /// Auto-serializes a packet, assuming all properties can be serialized with a binarywriter by default
        /// </summary>
        /// <param name="packet">The packet to be serialized</param>
        /// <param name="bw">The stream in which the properties should be serialized to</param>
        public static void AutoSerialize(this IAutoSerializePacket packet, BinaryWriter bw)
        {
            //Note: no safety check if type is not serializable with BinaryWriter by default!
            packet.GetType().GetProperties().Iterate(x =>
                                                         {
                                                             if (x.Name != "PacketSize")
                                                                 bw.Write((dynamic) x.GetValue(packet));
                                                         });
        }
    }
}
