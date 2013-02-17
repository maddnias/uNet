using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace uNet.Structures.Packets.Base
{
    /// <summary>
    /// Inherit this in your packet if you explicitly wish the packet to remain non-compressed
    /// </summary>
    interface INonCompressedPacket : IPacket
    {
    }
}
