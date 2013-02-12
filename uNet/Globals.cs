using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uNet.Server;
using uNet.Structures;

namespace uNet
{
    internal static class Globals
    {
        //settings
        public static ICryptoScheme CryptoScheme { get; set; }

        public static string Version = "0.0.1";
        public static uNetServer Server;
        public static List<short> ReservedPacketIDs = new List<short> { 0, 1};
    }
}
