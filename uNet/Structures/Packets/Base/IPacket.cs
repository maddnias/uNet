using System.IO;

namespace uNet.Structures.Packets.Base
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
        /// Returns the total size of the packet
        /// </summary>
        int PacketSize { get; }
        /// <summary>
        /// Serialize all data to be sent into the input buffer
        /// </summary>
        /// <param name="outputBuffer">Buffer to be filled with packet data</param>
        void SerializePacket(Stream outputBuffer);
        /// <summary>
        /// Populate all packet properties by reading from the inputBuffer
        /// </summary>
        /// <param name="inputBuffer">Buffer containing packet data</param>
        void DeserializePacket(Stream inputBuffer);
    }
}
