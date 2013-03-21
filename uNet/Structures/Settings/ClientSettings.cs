using System.Collections.Generic;
using uNet.Structures.Compression.Base;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings.Base;
using uNet.Utilities;

namespace uNet.Structures.Settings
{
    public class ClientSettings : OptionSet<uProtocolProcessor>
    {
        /// <summary>
        /// Server SSL certificate identity, required in order to authenticate connection
        /// </summary>
        public ServerCertificateIdentity SslServerCertIdentity;
        /// <summary>
        /// Enable/Disable SSL for connection
        /// </summary>
        public bool UseSsl { get; set; }

        public ClientSettings(List<IPacket> packetTable, ICompressor packetCompressor, bool useSsl, ServerCertificateIdentity? serverIdentity = null)
            : base(packetTable, packetCompressor)
        {
            UseSsl = useSsl;
            if(serverIdentity != null)
                SslServerCertIdentity = (ServerCertificateIdentity)serverIdentity;
        }
    }
}