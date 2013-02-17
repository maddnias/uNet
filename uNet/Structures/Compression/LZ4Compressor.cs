using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ4Sharp;
using uNet.Structures.Compression.Base;

namespace uNet.Structures.Compression
{
    public class LZ4Compressor : ICompressor
    {
        public short CompressionID { get { return 1; } }

        public byte[] CompressData(byte[] rawData)
        {
            return CompressionCalc.ShouldCompress(rawData) ? LZ4.Compress(rawData) : null;
        }

        public byte[] DecompressData(byte[] compressedData)
        {
            return LZ4.Decompress(compressedData);
        }
    }
}
