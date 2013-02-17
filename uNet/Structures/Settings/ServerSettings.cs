using System.Collections.Generic;
using uNet.Structures.Compression.Base;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings.Base;

namespace uNet.Structures.Settings
{
    public class ServerSettings : OptionSet
    {
        /// <summary>
        /// Local path for server to read SSL certificate from. You can automatically generate a SSL certificate using CertUtil
        /// </summary>
        public string SslCertLocation { get; set; }

        public ServerSettings()
            : base(null, null, false)
        {
            
        }

        public ServerSettings(List<IPacket> packetTable, ICompressor packetCompressor, bool useSsl, string certLocation = null)
            : base(packetTable, packetCompressor, useSsl)
        {
            SslCertLocation = certLocation;
        }
    }
}