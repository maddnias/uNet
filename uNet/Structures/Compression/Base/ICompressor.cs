namespace uNet.Structures.Compression.Base
{
    public interface ICompressor
    {
        /// <summary>
        /// This ID has to match both client and server side
        /// </summary>
        short CompressionID { get; }

        /// <summary>
        /// Compress raw data and return it to packet processor
        /// </summary>
        /// <param name="rawData">Raw packet data to be compressed</param>
        /// <returns>Compressed packet</returns>
        byte[] CompressData(byte[] rawData);

        /// <summary>
        /// Decompress packet and return it to packet processor
        /// </summary>
        /// <param name="compressedData">Data to be decompressed</param>
        /// <returns>Raw packet data</returns>
        byte[] DecompressData(byte[] compressedData);
    }
}
