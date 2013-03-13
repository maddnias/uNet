namespace uNet.Structures.Packets.Base
{
    /// <summary>
    /// Inherit this in your packet if you explicitly wish the packet to remain non-compressed
    /// </summary>
    interface INonCompressedPacket : IPacket
    {
    }
}
