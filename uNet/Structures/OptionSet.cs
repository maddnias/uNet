using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uNet.Structures
{
    public struct ServerCertificateIdentity
    {
        public string CertName;
        public string Thumbprint;
        public string KeyLength;

        /// <summary>
        /// Structure containing certificate data
        /// </summary>
        /// <param name="identityString">A string containing certificate data 
        /// with format: "Certificate name|Thumbprint|Key length".
        /// You can extract an identity string from a certificate automatically with CertUtil.exe</param>
        public ServerCertificateIdentity(string identityString)
        {
            CertName = "CN=" + identityString.Split('|')[0];
            Thumbprint = identityString.Split('|')[1];
            KeyLength = identityString.Split('|')[2];
        }
    }

    public class ClientSettings : OptionSet
    {
        /// <summary>
        /// Server SSL certificate identity, required in order to authenticate connection
        /// </summary>
        public ServerCertificateIdentity SSLServerCertIdentity;

        public ClientSettings(List<IPacket> packetTable, bool useSSL, ServerCertificateIdentity? serverIdentity = null)
            : base(packetTable, useSSL)
        {
            if(serverIdentity != null)
                SSLServerCertIdentity = (ServerCertificateIdentity)serverIdentity;
        }
    }

    public class ServerSettings : OptionSet
    {
        /// <summary>
        /// Local path for server to read SSL certificate from. You can automatically generate a SSL certificate using CertUtil
        /// </summary>
        public string SSLCertLocation { get; set; }

        public ServerSettings(List<IPacket> packetTable, bool useSSL, string certLocation = null)
            : base(packetTable, useSSL)
        {
            SSLCertLocation = certLocation;
        }
    }

    public abstract class OptionSet
    {
        /// <summary>
        /// Enable/Disable SSL for connection
        /// </summary>
        public bool UseSSL { get; set; }
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

        protected OptionSet(List<IPacket> packetTable, bool useSSL, int receiveBufferSize = 1024)
        {
            PacketTable = packetTable;
            UseSSL = useSSL;
            ReceiveBufferSize = receiveBufferSize;
        }
    }
}
