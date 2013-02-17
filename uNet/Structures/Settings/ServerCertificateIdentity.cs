namespace uNet.Structures.Settings
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
}