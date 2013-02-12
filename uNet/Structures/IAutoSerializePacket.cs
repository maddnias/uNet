namespace uNet.Structures
{
    /// <summary>
    /// High level interpretation of a data packet
    /// WARNING: May only be used if all properties in packet can be serialized with BinaryWriter by default
    /// </summary>
    public interface IAutoSerializePacket : IPacket { }
}
