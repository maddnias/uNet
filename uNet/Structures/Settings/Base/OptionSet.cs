using System.Collections.Generic;
using uNet.Structures.Compression.Base;
using uNet.Structures.Packets.Base;

namespace uNet.Structures.Settings.Base
{
    public abstract class OptionSet
    {
        /// <summary>
        /// Enable/Disable SSL for connection
        /// </summary>
        public bool UseSsl { get; set; }
        /// <summary>
        /// A list of IPacket containing all custom packets
        /// WARNING: List must be equal on both client and server side
        /// </summary>
        public List<IPacket> PacketTable { get; set; }
        /// <summary>
        /// Buffer size for receiving data
        /// Default = 1024
        /// </summary>
        public int ReceiveBufferSize { get; set; }
        /// <summary>
        /// The compression algorithm to use for packets
        /// </summary>
        public ICompressor PacketCompressor { get; set; }

        protected OptionSet(List<IPacket> packetTable, ICompressor packetCompressor, bool useSsl, int receiveBufferSize = 1024)
        {
            PacketTable = packetTable;
            UseSsl = useSsl;
            ReceiveBufferSize = receiveBufferSize;
            PacketCompressor = packetCompressor;
        }
    }
}
