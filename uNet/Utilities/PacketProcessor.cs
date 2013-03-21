using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using uNet.Structures.Events;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings.Base;

namespace uNet.Utilities
{
    public abstract class PacketProcessor
    {
        public OptionSet Settings;
        internal event EventHandler<PacketEventArgs> _onPacketSent;

        /// <summary>
        /// Call this method after sending a packet with SendPacket
        /// </summary>
        /// <param name="e">Event args</param>
        public void OnPacketSent(PacketEventArgs e)
        {
            if (_onPacketSent != null)
                _onPacketSent(this, e);
        }

        /// <summary>
        /// Send a packet to network stream
        /// </summary>
        /// <param name="packet">The packet to be sent</param>
        /// <param name="netStream">The stream to write the packet to</param>
        public abstract Task SendPacket(IPacket packet, Stream netStream);

        /// <summary>
        /// Parse a packet from raw byte array from ProcessData"/>
        /// </summary>
        /// <param name="rawData">Raw packet data</param>
        /// <returns>Parsed packet</returns>
        public abstract IPacket ParsePacket(byte[] rawData);

        /// <summary>
        /// Processes received data from network stream
        /// </summary>
        /// <param name="buffer">Buffer containing read data</param>
        /// <returns>Returns a list of IPackets parsed from the buffer</returns>
        public abstract List<IPacket> ProcessData(List<byte> buffer);
    }
}
