using System.Collections.Generic;
using uNet.Structures.Compression.Base;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings.Base;
using uNet.Utilities;

namespace uNet.Structures.Settings
{
    public class ServerSettings : OptionSet<uProtocolProcessor>
    {
        /// <summary>
        /// Local path for server to read SSL certificate from. You can automatically generate a SSL certificate using CertUtil
        /// </summary>
        public string SslCertLocation { get; set; }
        /// <summary>
        /// Enable/Disable SSL for connection
        /// </summary>
        public bool UseSsl { get; set; }

        public ServerSettings(List<IPacket> packetTable, ICompressor packetCompressor, bool useSsl, string certLocation = null)
            : base(packetTable, packetCompressor)
        {
            UseSsl = useSsl;
            SslCertLocation = certLocation;
        }
    }
}