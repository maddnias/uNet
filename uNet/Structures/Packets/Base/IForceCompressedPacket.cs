namespace uNet.Structures.Packets.Base
{
    /// <summary>
    /// Inherit this in your packet if you wish to force compression regardless of inflation
    /// </summary>
    interface IForceCompressedPacket : IPacket
    {
    }
}
