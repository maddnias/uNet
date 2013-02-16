using System.Collections.Generic;

namespace uNet
{
    internal static class Globals
    {
        public static string Version = "0.0.1";
        public static List<short> ReservedPacketIDs = new List<short> { 9998, 9999 };
    }
}
