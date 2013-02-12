namespace uNet.Structures
{
    public interface ICryptoScheme
    {
        int SchemeID { get; }
        byte[] EncryptData(byte[] plainData);
        byte[] DecryptData(byte[] encData);
    }
}
