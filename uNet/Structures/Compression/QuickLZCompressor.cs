using uNet.Compression;
using uNet.Structures.Compression.Base;

namespace uNet.Structures.Compression
{
    public class QuickLZCompressor : ICompressor
    {
        public short CompressionID { get { return 0; } }

        public byte[] CompressData(byte[] rawData)
        {
            return CompressionCalc.ShouldCompress(rawData) ? QuickLZ.compress(rawData, 1) : null;
        }

        public byte[] DecompressData(byte[] compressedData)
        {
            return QuickLZ.decompress(compressedData);
        }
    }
}
