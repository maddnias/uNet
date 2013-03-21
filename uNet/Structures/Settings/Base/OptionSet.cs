using System;
using System.Collections.Generic;
using uNet.Structures.Compression.Base;
using uNet.Structures.Packets.Base;
using uNet.Utilities;

namespace uNet.Structures.Settings.Base
{
    /// <summary>
    /// Do not use
    /// </summary>
    public abstract class OptionSet
    {
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
        public PacketProcessor Processor { get; set; }

        protected OptionSet(List<IPacket> packetTable, ICompressor packetCompressor, int receiveBufferSize = 1024)
        {
            PacketTable = packetTable;
            ReceiveBufferSize = receiveBufferSize;
            PacketCompressor = packetCompressor;
        }
    }

    public abstract class OptionSet<T> : OptionSet where T : PacketProcessor
    {
        protected OptionSet(List<IPacket> packetTable, ICompressor packetCompressor, int receiveBufferSize = 1024)
            : base(packetTable, packetCompressor, receiveBufferSize)
        {
            Processor = Activator.CreateInstance<T>();
        }
    }
}
