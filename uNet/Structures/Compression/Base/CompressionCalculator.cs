using System;
using System.Linq;

// Class by 0xDEADDEAD ( https://github.com/Dextrey )
namespace uNet.Structures.Compression.Base
{
    public static class CompressionCalc
    {
        private static double GetRandomityFactor(byte[] input)
        {
            double average = input.Aggregate(0.0, (current, b) => current + b);
            return (128.0 - Math.Abs((average / input.Length) - 128.0)) / 128.0;
        }

        public static bool ShouldCompress(byte[] input)
        {
            var inputSize = input.Length;
            var randomityFactor = GetRandomityFactor(input);

            if (inputSize > 0 && inputSize <= 50) return 0.1847588952 * Math.Log(inputSize, Math.E) + 0.1772008143 > randomityFactor;
            if (inputSize > 50 && inputSize <= 100) return 0.063030975 * Math.Log(inputSize, Math.E) + 0.629623763 > randomityFactor;
            if (inputSize > 100 && inputSize <= 200) return 0.038317199 * Math.Log(inputSize, Math.E) + 0.7453108135 > randomityFactor;
            if (inputSize > 200 && inputSize <= 400) return 0.021723425 * Math.Log(inputSize, Math.E) + 0.8329499575 > randomityFactor;

            return 0.0152141725 * Math.Log(inputSize, Math.E) + 0.8719141065 > randomityFactor;
        }
    }
}
