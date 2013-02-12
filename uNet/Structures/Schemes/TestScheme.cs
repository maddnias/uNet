using System.Linq;

namespace uNet.Structures.Schemes
{
    public class TestScheme : ICryptoScheme
    {
        public int SchemeID { get { return 1337; } }

        public byte[] EncryptData(byte[] plainData)
        {
            return plainData.Reverse().ToArray();
        }

        public byte[] DecryptData(byte[] encData)
        {
            return encData.Reverse().ToArray();
        }
    }
}
