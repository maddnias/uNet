using System.Collections.Generic;
using uNet.Structures.Compression.Base;
using uNet.Structures.Packets.Base;
using uNet.Structures.Settings.Base;

namespace uNet.Structures.Settings
{
    public class ClientSettings : OptionSet
    {
        /// <summary>
        /// Server SSL certificate identity, required in order to authenticate connection
        /// </summary>
        public ServerCertificateIdentity SslServerCertIdentity;

        public ClientSettings()
            : base(null, null, false)
        {
            
        }

        public ClientSettings(List<IPacket> packetTable, ICompressor packetCompressor, bool useSsl, ServerCertificateIdentity? serverIdentity = null)
            : base(packetTable, packetCompressor, useSsl)
        {
            if(serverIdentity != null)
                SslServerCertIdentity = (ServerCertificateIdentity)serverIdentity;
        }
    }
}