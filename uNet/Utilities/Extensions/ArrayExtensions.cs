using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace uNet.Utilities.Extensions
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Slice an array to get a specific part of it
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="arr">The array</param>
        /// <param name="idx">Index to initiate slicing from</param>
        /// <param name="count">Amount of elements to slice</param>
        /// <returns></returns>
        public static T[] Slice<T>(this T[] arr, int idx, int count)
        {
            return _Slice(arr, idx, count).ToArray();
        }

        private static IEnumerable<T> _Slice<T>(T[] arr, int idx, int count)
        {
            for (var i = idx; i < idx + count; i++)
                yield return arr[i];
        }

        /// <summary>
        /// Provides an easier and neater way to iterate an array
        /// </summary>
        /// <typeparam name="T">Array type</typeparam>
        /// <param name="iteratable">The array</param>
        /// <param name="act">Action to be invoked upon each element</param>
        public static void Iterate<T>(this T[] iteratable, Action<T> act)
        {
            foreach (var x in iteratable)
                act(x);
        }

        public static byte[] GetMd5Hash(this byte[] buffer)
        {
            return new MD5CryptoServiceProvider().ComputeHash(buffer);
        }
    }
}
