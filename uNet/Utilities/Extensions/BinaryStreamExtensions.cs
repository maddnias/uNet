using System.IO;

namespace uNet.Utilities.Extensions
{
    public static class BinaryStreamExtensions
    {
        //Thanks 0xDEADDEAD
        public static void WriteEncodedInteger(this BinaryWriter bw, ulong integer)
        {
            do
            {
                var part = (byte)(0x7F & integer);
                part = (integer >> 7 == 0) ? part : (byte)(part | 0x80);
                bw.Write(part);
                integer = integer >> 7;
            } while (integer != 0);
        }

        //Thanks 0xDEADDEAD
        public static ulong ReadEncodedInteger(this BinaryReader br)
        {
            ulong value = 0;
            var shift = 0;
            do
            {
                var part = br.ReadByte();
                value |= (ulong)(0x7F & part) << shift;
                if ((part & 0x80) == 0) return value;
                shift += 7;
            } while (true);
        }
    }
}
