using System.IO;

namespace uNet.Structures
{
    /// <summary>
    /// High level interpretation of a data packet
    /// </summary>
    public interface IPacket
    {
        /// <summary>
        /// Each packet is required to have a unique ID, reserved IDs range from 9998-9999
        /// </summary>
        short ID { get; }
        /// <summary>
        /// Serialize all data to be sent into the BinaryWriter provided in the arguments
        /// </summary>
        /// <param name="writer"></param>
        void SerializePacket(BinaryWriter writer);
        /// <summary>
        /// Populate all packet properties by reading from the BinaryReader
        /// </summary>
        /// <param name="reader"></param>
        void DeserializePacket(BinaryReader reader);
    }
}
